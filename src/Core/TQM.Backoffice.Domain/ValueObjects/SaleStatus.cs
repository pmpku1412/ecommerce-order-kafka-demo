namespace TQM.Backoffice.Domain.ValueObjects;

public class SaleStatuses
{
    public string Value { get; private set; } = string.Empty;
    public SaleStatuses(string value) => Value = value;
    /// <summary>
    /// ตรวจทานแล้ว
    /// </summary>
    /// <returns></returns>
    public static SaleStatuses S => new("S");
    /// <summary>
    /// รอตรวจทาน
    /// </summary>
    /// <returns></returns>
    public static SaleStatuses W => new("W");
    /// <summary>
    /// ยืนยันโดย Operation
    /// </summary>
    /// <returns></returns>
    public static SaleStatuses O => new("O");
    /// <summary>
    /// ยกเลิกโดย SALE
    /// </summary>
    /// <returns></returns>
    public static SaleStatuses X => new("X");
    /// <summary>
    /// อนุมัติ
    /// </summary>
    /// <returns></returns>
    public static SaleStatuses C => new("C");
    /// <summary>
    /// ยกเลิก
    /// </summary>
    /// <returns></returns>
    public static SaleStatuses V => new("V");
}
