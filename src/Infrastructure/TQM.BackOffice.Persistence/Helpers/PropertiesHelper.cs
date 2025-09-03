namespace TQM.BackOffice.Persistence.Helpers;

internal class PropertiesHelper
{
    /// <summary>
    /// Get Property name indicate that value has not null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<Tuple<string, object?>> GetNotNullProps<T>(T obj)
    {
        return typeof(T).GetProperties()
            .Select(x => Tuple.Create(x.Name, x.GetValue(obj)))
            .Where(x => x.Item2 != null)
            .ToList();
    }

    public static List<Tuple<string, object?, object?>> GetNotNullProps_ForUpdate<T>(T obj, T objDefault)
    {
        // var a = typeof(T).GetProperties()
        //     .Select(x => Tuple.Create(x.Name, x.GetValue(obj), x.GetValue(objDefault)))
        //     .Where(x => (x.Item2 != x.Item3))
        //     .ToList();

        return typeof(T).GetProperties()
            .Select(x => Tuple.Create(x.Name, x.GetValue(obj), x.GetValue(objDefault)))
            .Where(x => Object.Equals(x.Item2,x.Item3) == false)
            .ToList();
    }
}
