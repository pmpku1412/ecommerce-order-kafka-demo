namespace TQM.Backoffice.Domain.ValueObjects;

public class ClaimType
{
    public string Value { get; private set; } = string.Empty;

    public ClaimType(string value) => Value = value;

    public static ClaimType StaffId => new("StaffId");
    public static ClaimType StaffCode => new("StaffCode");
    public static ClaimType DbLogin => new("DbLogin");
    public static ClaimType DepartmentId => new("DepartmentId");
    public static ClaimType StaffDepartmentId => new("StaffDepartmentId");
    public static ClaimType Permissions => new("Permissions");
    public static ClaimType Role => new("Role");
    public static ClaimType BranchId => new("BranchId");
    public static ClaimType BranchCode => new("BranchCode");
    public static ClaimType SessionId => new("SessionId");
}