namespace TQM.Backoffice.Domain.ValueObjects;

public class ErrorHandler
{
    public string ErrorCode { get; private set; } = string.Empty;
    public string ErrorGroup { get; private set; } = string.Empty;
    public string ErrorMessage { get; private set; } = string.Empty;

    private ErrorHandler(string errorCode, string errorGroup, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorGroup = errorGroup;
        ErrorMessage = errorMessage;
    }

    public static ErrorHandler E1025 => new("1025", "Internal Error", "SMC ไม่ถูกต้อง");
    public static ErrorHandler E1026 => new("1026", "Internal Error", "ข้อจำกัดที่อนุญาติ ........");
}
