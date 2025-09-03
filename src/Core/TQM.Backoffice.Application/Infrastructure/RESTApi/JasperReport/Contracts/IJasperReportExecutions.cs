using TQM.Backoffice.Application.DTOs.Common;
namespace TQM.Backoffice.Application.RESTApi.JasperReport.Contracts;
public interface IJasperReportExecutions
{
    Task<ReportExecutionServiceInfo> SubmitReportExecutions(ReportExecutionRequest request);
    Task<string> JasperReportExecution(ReportExecutionRequest request);
}
