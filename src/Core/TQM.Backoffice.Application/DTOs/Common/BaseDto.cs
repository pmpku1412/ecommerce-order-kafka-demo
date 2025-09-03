namespace TQM.Backoffice.Application.DTOs.Common;

public class BaseDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    //public bool ValidateOnly { get; set; } = false;
    // public string? CreateUserId { get; set; }
    // public DateTime? CreateDateTime { get; set; }
    // public string? LastUpdateUserId { get; set; }
    // public DateTime? LastUpdateDateTime { get; set; }
}

public class BaseDtoCompany : BaseDto
{
    public int CompanyId { get; set; } = 0;     // For case that Master data Rely on CompanyId
}
