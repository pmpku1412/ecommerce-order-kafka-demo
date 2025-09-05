namespace TQM.Backoffice.Domain.Events;

public class StockUpdatedEvent
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public int QuantityChanged { get; set; }
    public string Reason { get; set; } = string.Empty; // "ORDER_CREATED", "MANUAL_ADJUSTMENT", etc.
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public int? OrderId { get; set; }
}
