using System.Linq;

namespace TQM.Backoffice.Application.Helpers;

public static class CheckNull
{
    public static bool CheckNullProp<T>(this T prop)
    {
        try
        {
            if (prop is null) return false;
            var PropertyProp = prop.GetType().GetProperties().FirstOrDefault();
            if (PropertyProp is not null)
            {
                var SubProp = PropertyProp.GetValue(prop);
                if (SubProp is not null)
                {
                    foreach (var _ in SubProp.GetType().GetProperties().Where(item => item.GetValue(SubProp) != null).Select(item => new { }))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
        catch
        {
            return true;
        }
    }
    public static bool CheckNullProp<T>(this T prop, string propName)
    {
        try
        {
            if (prop is null) return false;
            var PropertyProp = prop.GetType().GetProperties().FirstOrDefault(item => item.Name == propName);
            if (PropertyProp is not null)
            {
                var SubProp = PropertyProp.GetValue(prop);
                if (SubProp is not null)
                {
                    foreach (var _ in SubProp.GetType().GetProperties().Where(item => item.GetValue(SubProp) != null).Select(item => new { }))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
        catch
        {
            return true;
        }
    }
}
