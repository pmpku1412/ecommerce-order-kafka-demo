namespace TQM.Backoffice.Application.DTOs.Common;

// Info from Report Execution response then we want to send to UI
public class ReportExecutionServiceInfo
{
    public string Status { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public string ExportId { get; set; } = string.Empty;
    public string ReportUrl { get; set; } = string.Empty;
}