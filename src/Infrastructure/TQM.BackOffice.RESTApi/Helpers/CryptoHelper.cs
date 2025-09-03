using System.Web;

namespace TQM.BackOffice.RESTApi.Helpers;

public static class CryptoHelper
{
    public static string ConvertToQueryString<T>(T data)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(data);

            var properties = data.GetType().GetProperties()
                            .Where(x => x.GetValue(data, null) != null)
                            .Select(x => x.Name + "=" + HttpUtility.UrlEncode(x.GetValue(data, null)?.ToString()));

            // queryString will be set to "Id=1&State=26&Prefix=f&Index=oo"                  
            return string.Join("&", properties.ToArray());
        }
        catch (Exception) { return string.Empty; }
    }
}
