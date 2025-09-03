namespace TQM.BackOffice.Persistence.Helpers
{
    public class IntegerConversion
    {
        /// <summary>
        /// Null To Zero function (Access)
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public int Ntz(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }
            else
            {
                int outInt;
                if (int.TryParse(input, out outInt))
                {
                    return outInt;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}