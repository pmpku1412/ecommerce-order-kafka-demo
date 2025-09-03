namespace TQM.Backoffice.Application.DTOs.Task;

public class ActionAlert
{
    public string Sale { get; set; } = string.Empty;
    public long PeriodId { get; set; }
    public string SaleBookCode { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public DateTime SaleDate { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string SaleStaff { get; set; } = string.Empty;
    public string DepartmentCode { get; set; } = string.Empty;
    public long SaleId { get; set; }
    public string PlateId { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string RequestRemark { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime ActionDate { get; set; }
    public string ActionStatus { get; set; } = string.Empty;
    public string ActionRemark { get; set; } = string.Empty;
    public DateTime NextDueDate { get; set; }
    public int NextSequence { get; set; }
    public long ActionId { get; set; }
    public long ResultId { get; set; }
    public int Installment { get; set; }
    public long JobId { get; set; }
    public string PhoneNo { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public long RouteId { get; set; }
    public long RequestUserId { get; set; }
    public int AcSequence { get; set; }
    public string RequestName { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string UserMonitor { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;
    public string SupplierCode { get; set; } = string.Empty;
    public DateTime PolicyDate { get; set; }
    public decimal PaidAmount { get; set; }
}