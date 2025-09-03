using MediatR;
using Microsoft.AspNetCore.Mvc;
using TQM.Backoffice.Application.Commands.Orders;
using TQM.Backoffice.Application.Contracts.Persistence;
using TQM.Backoffice.Application.DTOs.Orders;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ApiControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public OrdersController(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    /// <param name="request">Order creation request</param>
    /// <returns>Created order details</returns>
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand
        {
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            Items = request.Items
        };

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <returns>Order details</returns>
    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(string orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            return NotFound($"Order with ID {orderId} not found");
        }

        var response = new OrderResponse
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            Items = order.Items.Select(i => new OrderItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };

        return Ok(response);
    }

    /// <summary>
    /// Get all orders
    /// </summary>
    /// <returns>List of orders</returns>
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAllOrders()
    {
        var orders = await _orderRepository.GetAllAsync();
        var response = orders.Select(order => new OrderResponse
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            Items = order.Items.Select(i => new OrderItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        }).ToList();

        return Ok(response);
    }
}
