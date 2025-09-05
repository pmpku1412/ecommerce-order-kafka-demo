using TQM.Backoffice.Core.Application.Contracts.Infrastructure;
using TQM.Backoffice.Core.Application.Contracts.Persistence;
using TQM.Backoffice.Core.Application.DTOs.Orders.Request;
using TQM.Backoffice.Core.Application.DTOs.Orders.Response;
using TQM.Backoffice.Domain.Entities;

namespace TQM.BackOffice.Persistence.Services;

public class OrderService : IOrderService
{
    private static List<Order> _orders = new List<Order>();
    private static int _nextOrderId = 1;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IProductService _productService;

    public OrderService(IKafkaProducer kafkaProducer, IProductService productService)
    {
        _kafkaProducer = kafkaProducer;
        _productService = productService;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        // Validate products and calculate total
        var orderItems = new List<OrderItemResponse>();
        decimal totalAmount = 0;

        foreach (var item in request.OrderItems)
        {
            var product = _productService.GetProductById(item.ProductId);
            if (product == null)
                throw new Exception($"Product {item.ProductId} not found");

            if (!_productService.CheckStockAvailability(item.ProductId, item.Quantity))
                throw new Exception($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {item.Quantity}");

            var orderItem = new OrderItemResponse
            {
                Id = orderItems.Count + 1,
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                TotalPrice = product.Price * item.Quantity,
                ProductImage = product.ImageUrl,
                ProductDescription = product.Description
            };

            orderItems.Add(orderItem);
            totalAmount += orderItem.TotalPrice;

            // Reduce stock and publish Kafka event
            var previousStock = product.Stock;
            _productService.ReduceStock(item.ProductId, item.Quantity);
            
            // Get updated product to get correct new stock
            var updatedProduct = _productService.GetProductById(item.ProductId);
            var newStock = updatedProduct?.Stock ?? 0;

            // Publish stock updated event to Kafka
            await _kafkaProducer.PublishStockUpdatedFromOrderAsync(
                product.Id, 
                product.Name, 
                previousStock, 
                newStock, 
                _nextOrderId
            );
        }

        // Create order
        var order = new Order
        {
            Id = _nextOrderId++,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            TotalAmount = totalAmount,
            Status = "Pending",
            CreatedDate = DateTime.Now,
            ShippingAddress = request.ShippingAddress,
            PaymentMethod = request.PaymentMethod,
            OrderItems = orderItems.Select(oi => new OrderItem
            {
                Id = oi.Id,
                OrderId = _nextOrderId - 1,
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                ProductImage = oi.ProductImage,
                ProductDescription = oi.ProductDescription
            }).ToList()
        };

        _orders.Add(order);

        var response = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedDate = order.CreatedDate,
            UpdatedDate = order.UpdatedDate,
            ShippingAddress = order.ShippingAddress,
            PaymentMethod = order.PaymentMethod,
            OrderItems = orderItems
        };

        return await Task.FromResult(response);
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(int orderId)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null) return null;

        var response = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedDate = order.CreatedDate,
            UpdatedDate = order.UpdatedDate,
            ShippingAddress = order.ShippingAddress,
            PaymentMethod = order.PaymentMethod,
            OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                TotalPrice = oi.TotalPrice,
                ProductImage = oi.ProductImage,
                ProductDescription = oi.ProductDescription
            }).ToList()
        };

        return await Task.FromResult(response);
    }

    public async Task<List<OrderResponse>> GetOrdersByCustomerIdAsync(string customerId)
    {
        var orders = _orders.Where(o => o.CustomerId == customerId).ToList();
        var responses = orders.Select(order => new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedDate = order.CreatedDate,
            UpdatedDate = order.UpdatedDate,
            ShippingAddress = order.ShippingAddress,
            PaymentMethod = order.PaymentMethod,
            OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                TotalPrice = oi.TotalPrice,
                ProductImage = oi.ProductImage,
                ProductDescription = oi.ProductDescription
            }).ToList()
        }).ToList();

        return await Task.FromResult(responses);
    }

    public async Task<List<OrderResponse>> GetAllOrdersAsync()
    {
        var responses = _orders.Select(order => new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedDate = order.CreatedDate,
            UpdatedDate = order.UpdatedDate,
            ShippingAddress = order.ShippingAddress,
            PaymentMethod = order.PaymentMethod,
            OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                TotalPrice = oi.TotalPrice,
                ProductImage = oi.ProductImage,
                ProductDescription = oi.ProductDescription
            }).ToList()
        }).ToList();

        return await Task.FromResult(responses);
    }

    public async Task<OrderResponse> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
            throw new Exception($"Order {orderId} not found");

        order.Status = status;
        order.UpdatedDate = DateTime.Now;

        var response = new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedDate = order.CreatedDate,
            UpdatedDate = order.UpdatedDate,
            ShippingAddress = order.ShippingAddress,
            PaymentMethod = order.PaymentMethod,
            OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                UnitPrice = oi.UnitPrice,
                Quantity = oi.Quantity,
                TotalPrice = oi.TotalPrice,
                ProductImage = oi.ProductImage,
                ProductDescription = oi.ProductDescription
            }).ToList()
        };

        return await Task.FromResult(response);
    }
}
