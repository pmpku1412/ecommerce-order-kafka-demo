using TQM.Backoffice.Core.Application.DTOs.Orders.Request;
using TQM.Backoffice.Core.Application.DTOs.Orders.Response;

namespace TQM.Backoffice.Core.Application.Contracts.Persistence;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderResponse?> GetOrderByIdAsync(int orderId);
    Task<List<OrderResponse>> GetOrdersByCustomerIdAsync(string customerId);
    Task<List<OrderResponse>> GetAllOrdersAsync();
    Task<OrderResponse> UpdateOrderStatusAsync(int orderId, string status);
}
