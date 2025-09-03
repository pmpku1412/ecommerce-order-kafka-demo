namespace TQM.Backoffice.Domain.ValueObjects;

public class ActionStatuses
{
    public string Value { get; private set; } = string.Empty;
    public ActionStatuses(string value) => Value = value;
    /// <summary>
    /// รอตรวจสอบ
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses B => new("B");
    /// <summary>
    /// เลื่อนหลัง-หลังจัดส่ง
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses D => new("D");
    /// <summary>
    /// เลื่อนก่อน-ก่อนจัดส่ง
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses E => new("E");
    /// <summary>
    /// งานมีปัญหา
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses F => new("F");
    /// <summary>
    /// หัวหน้าทีมตรวจสอบ
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses K => new("K");
    /// <summary>
    /// เลื่อน-หลังถ่ายรูป
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses L => new("L");
    /// <summary>
    /// เลื่อน-ก่อนปฏิบัติงาน
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses O => new("O");
    /// <summary>
    /// เลื่อน-หลังปฏิบัติงาน
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses P => new("P");
    /// <summary>
    /// Sale Manager ทานคำตอบ/รับรอง
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses R => new("R");
    /// <summary>
    /// Sale ตอบ/ตั้ง จาก WEB
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses S => new("S");
    /// <summary>
    /// เลื่อน-ก่อนถ่ายรูป
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses T => new("T");
    /// <summary>
    /// รอดำเนินการ
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses W => new("W");
    /// <summary>
    /// ยกเลิก
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses X => new("X");
    /// <summary>
    /// ดำเนินการแล้ว
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses Y => new("Y");
    /// <summary>
    /// ยกเลิก โดยส่วนงานควบคุมเครดิต
    /// </summary>
    /// <returns></returns>
    public static ActionStatuses Z => new("Z");
}
