namespace TQM.BackOffice.Persistence.Helpers
{
    public static class DecimalConversion
    {
        /// <summary>
        /// Null To Zero function (Access)
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static Decimal Ntz(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }
            else
            {
                Decimal outInt;
                if (Decimal.TryParse(input, out outInt))
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