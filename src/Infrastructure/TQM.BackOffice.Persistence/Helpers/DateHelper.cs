namespace TQM.BackOffice.Persistence.Helpers
{
    public class DateHelper
    {
        /// <summary>
        /// Sub String with StartPosition no that refer human number instead of index
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static DateTime CsDate(string inputDate)
        {
            return new DateTime(int.Parse(inputDate.Substring(0,4)),int.Parse(inputDate.Substring(4,2)),int.Parse(inputDate.Substring(6,2)));
        }
    }
}