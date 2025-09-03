using MediatR;
using Microsoft.Extensions.Logging;
using TQM.Backoffice.Application.Commands.Orders;
using TQM.Backoffice.Application.Contracts.Infrastructure;
using TQM.Backoffice.Application.Contracts.Persistence;
using TQM.Backoffice.Application.DTOs.Orders;
using TQM.Backoffice.Application.Exceptions;
using TQM.Backoffice.Domain.Entities;
using TQM.Backoffice.Domain.Events;

namespace TQM.Backoffice.Application.Commands.Orders;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IKafkaProducer kafkaProducer,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating order for customer: {CustomerId}", request.CustomerId);

            // Validate products and calculate total
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new NotFoundException("Product", item.ProductId);
                }

                if (product.StockQuantity < item.Quantity)
                {
                    throw new BadRequestException($"Insufficient stock for product {product.ProductName}. Available: {product.StockQuantity}, Requested: {item.Quantity}");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;
            }

            // Create order
            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                Items = orderItems
            };

            var createdOrder = await _orderRepository.CreateAsync(order);

            // Publish event to Kafka
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = createdOrder.OrderId,
                CustomerId = createdOrder.CustomerId,
                CustomerName = createdOrder.CustomerName,
                OrderDate = createdOrder.OrderDate,
                TotalAmount = createdOrder.TotalAmount,
                Items = createdOrder.Items.Select(i => new OrderItemEvent
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _kafkaProducer.PublishAsync("orders", createdOrder.OrderId, orderEvent);

            _logger.LogInformation("Order created successfully: {OrderId}", createdOrder.OrderId);

            // Return response
            return new OrderResponse
            {
                OrderId = createdOrder.OrderId,
                CustomerId = createdOrder.CustomerId,
                CustomerName = createdOrder.CustomerName,
                OrderDate = createdOrder.OrderDate,
                TotalAmount = createdOrder.TotalAmount,
                Status = createdOrder.Status.ToString(),
                Items = createdOrder.Items.Select(i => new OrderItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for customer: {CustomerId}", request.CustomerId);
            throw;
        }
    }
}
