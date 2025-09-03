namespace TQM.Backoffice.Application.Responses;

public class BaseResponse<T>
{
    public decimal Id { get; set; }
    public bool Success { get; set; } = false;
    public string Message { get; set; } = "unexpected error";
    public List<string> Errors { get; set; } = new List<string>();
    public T? ResponseObject { get; set; }
}
