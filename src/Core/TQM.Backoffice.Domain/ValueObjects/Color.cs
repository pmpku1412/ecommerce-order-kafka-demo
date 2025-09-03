namespace TQM.Backoffice.Domain.ValueObjects;

public class Color
{
    public string Code { get; private set; } = "#000000";
    private Color(string code) => Code = code;

    public static Color White => new("#FFFFFF");
    public static Color Red => new("#FF5733");
    public static Color Orange => new("#FFC300");
    public static Color Yellow => new("#FFFF66");
    public static Color Green => new("#CCFF99 ");
    public static Color Blue => new("#6666FF");
    public static Color Purple => new("#9966CC");
    public static Color Grey => new("#999999");
}
