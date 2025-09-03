namespace TQM.Backoffice.Application.DTOs.Common;

public class BaseRequest
{
    public int CompanyId { get; set; } = 0;     // For case that Master data Rely on CompanyId
}
