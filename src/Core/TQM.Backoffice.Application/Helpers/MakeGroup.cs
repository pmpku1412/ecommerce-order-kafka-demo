namespace TQM.Backoffice.Application.Helpers ; 

    public static class MakeGroup 
    {
        public static List<List<T>> MakeGroupOf<T>(this List<T> list, int groupSize)
        {
            return list
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / groupSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
