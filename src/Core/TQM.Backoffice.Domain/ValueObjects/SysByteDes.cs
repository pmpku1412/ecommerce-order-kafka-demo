namespace TQM.Backoffice.Domain.ValueObjects;

public class SysByteDes
{
    public string ColumnName { get; private set; } = string.Empty;
    public SysByteDes(string columnName) => ColumnName = columnName;

    public static SysByteDes CORPSTATUS => new("CORPSTATUS");
    public static SysByteDes PAYMENTSTATUS => new("PAYMENTSTATUS");
    public static SysByteDes INSURETYPE => new("INSURETYPE");
    public static SysByteDes SALETYPE => new("SALETYPE");
    public static SysByteDes INSURELEVEL => new("INSURELEVEL");
    public static SysByteDes CARUSAGE => new("USAGE");
    public static SysByteDes BODYTYPE => new("BODYTYPE");
    public static SysByteDes CARGEAR => new("GEAR");
    public static SysByteDes TAKEPHOTO => new("TAKEPHOTO");
    public static SysByteDes DRIVERLICENSE => new("DRIVERLICENSE");
    public static SysByteDes POLICYSTATUS => new("POLICYSTATUS");
    public static SysByteDes PRBSTATUS => new("PRBSTATUS");
    public static SysByteDes SALEPIORITY => new("SALEPIORITY");
    public static SysByteDes SALESTATUS => new("SALESTATUS");
    public static SysByteDes PREFIX => new("PREFIX");
    public static SysByteDes BANKACCOUNTTYPE => new("BANKACCOUNTTYPE");
    public static SysByteDes PAYINTYPE => new("PAYINTYPE");
    public static SysByteDes CARDTYPE => new("CARDTYPE");
    public static SysByteDes ACTIONSTATUS => new("ACTIONSTATUS");
}
