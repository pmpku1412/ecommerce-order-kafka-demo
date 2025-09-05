using TQM.Backoffice.Application.DTOs.Common;

namespace TQM.Backoffice.Core.Application.DTOs.Orders.Request;

public class CreateOrderRequest : BaseRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public List<CreateOrderItemRequest> OrderItems { get; set; } = new List<CreateOrderItemRequest>();
}

public class CreateOrderItemRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
