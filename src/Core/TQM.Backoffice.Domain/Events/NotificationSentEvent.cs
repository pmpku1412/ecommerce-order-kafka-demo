namespace TQM.Backoffice.Domain.Events;

public class NotificationSentEvent
{
    public int OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty; // "ORDER_CONFIRMATION", "SHIPPING_UPDATE", etc.
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty; // "EMAIL", "SMS", "PUSH"
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime SentDate { get; set; } = DateTime.Now;
}
