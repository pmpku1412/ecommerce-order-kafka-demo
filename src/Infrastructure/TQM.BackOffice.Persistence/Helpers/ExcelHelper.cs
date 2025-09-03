using System.Globalization;

namespace TQM.BackOffice.Persistence.Helpers
{
    public class ExcelHelper
    {
        public static int ColumnLetterToColumnIndex(string columnLetter)
        {
            columnLetter = columnLetter.ToUpper();
            int sum = 0;

            for (int i = 0; i < columnLetter.Length; i++)
            {
                sum *= 26;
                sum += (columnLetter[i] - 'A' + 1);
            }
            return sum;
        }

        public static string ColumnIndexToColumnNumber(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
    
        //public static Boolean DateParseToServer(string _DateTimeStr, out DateTime _outDateTime)
        //{
        //    DateTime _date = DateTime.Now;
        //    try
        //    {
        //        string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

        //        int indexOfSlash_1_Format = sysFormat.IndexOfNth("/",0);
        //        int indexOfSlash_2_Format = sysFormat.IndexOfNth("/",1);
        //        string FirstPart_Format = sysFormat.Substring(0,indexOfSlash_1_Format);
        //        string SecondPart_Format = sysFormat.Substring(indexOfSlash_1_Format + 1,indexOfSlash_2_Format - indexOfSlash_1_Format - 1);
        //        string ThirdPart_Format = sysFormat.Substring(indexOfSlash_2_Format + 1).Trim().Split(null)[0].Trim();



        //        int indexOfSlash_1 = _DateTimeStr.IndexOfNth("/",0);
        //        int indexOfSlash_2 = _DateTimeStr.IndexOfNth("/",1);

        //        string FirstPart = _DateTimeStr.Substring(0,indexOfSlash_1);
        //        string SecondPart = _DateTimeStr.Substring(indexOfSlash_1 + 1,indexOfSlash_2 - indexOfSlash_1 - 1);
        //        string ThirdPart = _DateTimeStr.Substring(indexOfSlash_2 + 1).Trim().Split(null)[0].Trim();

        //        // Type 1 : server date M/d/yyyy

        //        int day = 0, month = 0, year = 0;

        //        switch (FirstPart_Format.ToLower().Substring(0,1))
        //        {
        //            case "d" : { day = int.Parse(FirstPart); } break;
        //            case "m" : { month = int.Parse(FirstPart); } break;
        //            case "y" : { year = int.Parse(FirstPart); } break;
        //        }

        //        switch (SecondPart_Format.ToLower().Substring(0,1))
        //        {
        //            case "d" : { day = int.Parse(SecondPart); } break;
        //            case "m" : { month = int.Parse(SecondPart); } break;
        //            case "y" : { year = int.Parse(SecondPart); } break;
        //        }

        //        switch (ThirdPart_Format.ToLower().Substring(0,1))
        //        {
        //            case "d" : { day = int.Parse(ThirdPart); } break;
        //            case "m" : { month = int.Parse(ThirdPart); } break;
        //            case "y" : { year = int.Parse(ThirdPart); } break;
        //        }

        //        _outDateTime = new DateTime(year, month, day);
        //    }
        //    catch (Exception)
        //    {
        //        _outDateTime = DateTime.Now;

        //        return false;
        //    }

        //    return true;
        //}
    
    }
}