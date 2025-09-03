using MediatR;
using TQM.Backoffice.Application.DTOs.Orders;

namespace TQM.Backoffice.Application.Commands.Orders;

public class CreateOrderCommand : IRequest<OrderResponse>
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
