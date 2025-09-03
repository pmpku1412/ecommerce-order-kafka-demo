namespace TQM.BackOffice.Persistence.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Sub String with StartPosition no that refer human number instead of index
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns></returns>
        public static string XSubstring(this string @this,int StartPosition,int Length = -1)
        {
            int UseIndex = ((StartPosition -1) < @this.Length) ? StartPosition -1 : @this.Length;

            if (Length == -1)
            {
                // Not Assign Length
                return @this.Substring(UseIndex);
            }
            else
            {
                string tempString = @this.Substring(UseIndex);

                if (tempString.Length > Length)
                    return @this.Substring(UseIndex,Length);
                else
                    return @this.Substring(UseIndex);
            }
        }

        /// <summary>
        /// Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right(this string value, int length)
        {
            if (String.IsNullOrEmpty(value)) return string.Empty;

            return value.Length <= length ? value : value.Substring(value.Length - length);
        }

        /// <summary>
        /// Get substring of specified number of characters on the Left.
        /// </summary>
        public static string Left(this string value, int length)
        {
            if (String.IsNullOrEmpty(value)) return string.Empty;

            return value.Length <= length ? value : value.Substring(0, length);
        }

        //public static string getValueFromStatementFormat(this string @this, List<StatementFormatDetail> InfoData, string ColumnName)
        //{
        //    StatementFormatDetail? item = InfoData.Where(x => x.ColumnName == ColumnName).FirstOrDefault();
        //    if (item != null)
        //    {
        //        if (item.RecordType == "D")
        //        {
        //            int StartPosition = item.StartPosition ?? 0;
        //            int Length = item.LengthSize ?? 0;

        //            return @this.XSubstring(StartPosition, Length); 
        //        }
        //        else
        //        {
        //            return @this;
        //        }
        //    }
        //    else
        //    {
        //        return @this;
        //    }          
        //}

        //public static string getValueFromStatementFormat_Array(this string[] @this, List<StatementFormatDetail> InfoData, string FieldName)
        //{
        //    //StatementFormatDetail? item = InfoData.Where(x => (x.FieldName == FieldName)).FirstOrDefault();
        //    StatementFormatDetail? item = InfoData.Where(x => (x.FieldName == FieldName) && (x.ColumnNo != "00000") ).FirstOrDefault();
        //    if (item != null)
        //    {
        //        if (item.RecordType == "F")
        //        {
        //            int? StartPosition = item.StartPosition;
        //            int? Length = item.LengthSize;
                                      
        //            if ((StartPosition.HasValue) && (Length.HasValue))
        //            {
        //                // Fix to have only 1 column reference
        //                int ColumnIndex = int.Parse(item.ColumnNo) - 1;
        //                return @this[ColumnIndex].XSubstring(StartPosition.Value,Length.Value);
        //            }
        //            else
        //            {
        //                int ColumnIndex = 0;

        //                if (item.ColumnNo.Contains("||"))
        //                {
        //                    string[] columnSplit = item.ColumnNo.Split("||");

        //                    string result = "";
        //                    foreach (string itemCol in columnSplit)
        //                    {
        //                        ColumnIndex = int.Parse(itemCol) - 1;

        //                        if (@this.Count() > ColumnIndex)
        //                        {
        //                            if (@this[ColumnIndex] != "")
        //                            {
        //                                result =  @this[ColumnIndex];
        //                                break;
        //                            }
        //                        }
        //                    }

        //                    return result;
        //                }
        //                else
        //                {                           
        //                    ColumnIndex = int.Parse(item.ColumnNo) - 1;
        //                    if (@this.Count() > ColumnIndex)
        //                        return @this[ColumnIndex];
        //                    else
        //                        return "";
        //                }                        
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"ไม่พบการตั้งค่า {FieldName}");
        //            //return "";
        //        }
        //    }
        //    else
        //    {
        //        return "";
        //    }          
        //}

        //public static StatementFormatDetail? getFormatDetailSplit_Array(string[] arrayList, List<StatementFormatDetail> InfoData, string FieldName)
        //{
        //    StatementFormatDetail? item = InfoData.Where(x => (x.FieldName == FieldName)).FirstOrDefault();
        //    StatementFormatDetail? item2 = new StatementFormatDetail(item);

        //    if (item != null)
        //    {
        //        if (item.RecordType == "F")
        //        {
        //            if (item.ColumnNo.Contains("||"))
        //            {
        //                int ColumnIndex = 0;
        //                int SplitIndex = 0;
        //                string[] columnSplit = item.ColumnNo.Split("||");

        //                string result = "";
        //                foreach (string itemCol in columnSplit)
        //                {
        //                    ColumnIndex = int.Parse(itemCol) - 1;

        //                    if (arrayList.Count() > ColumnIndex)
        //                    {
        //                        if (arrayList[ColumnIndex] != "")
        //                        {
        //                            result =  arrayList[ColumnIndex];
        //                            break;
        //                        }
        //                    }
        //                    SplitIndex += 1;
        //                }

        //                item2.ColumnName = item.ColumnName.Split("||")[SplitIndex];
        //                item2.ColumnNo = item.ColumnNo.Split("||")[SplitIndex];
        //                item2.DataFormat = (item.DataFormat ?? "").Split("||")[SplitIndex];

        //                return item2;
        //            }
        //            else
        //            {
        //                return item;
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"ไม่พบการตั้งค่า {FieldName}");
        //            //return "";
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }          
        //}


        //public static StatementFormatDetail? getFieldSetupInfo(List<StatementFormatDetail> InfoData, string FieldName)
        //{
        //    return InfoData.Where(x => (x.FieldName == FieldName)).FirstOrDefault();                
        //}

        //public static int IndexOfNth(this string str, string value, int nth = 0)
        //{
        //    if (nth < 0)
        //        throw new ArgumentException("Can not find a negative index of substring in string. Must start with 0");
            
        //    int offset = str.IndexOf(value);
        //    for (int i = 0; i < nth; i++)
        //    {
        //        if (offset == -1) return -1;
        //        offset = str.IndexOf(value, offset + 1);
        //    }
            
        //    return offset;
        //}

        public static bool IsNumeric(this string text) => double.TryParse(text, out _);

        // https://stackoverflow.com/questions/1968049/how-to-separate-character-and-number-part-from-string
        public static IEnumerable<string> SplitAlpha(string input)
        {
            var words = new List<string> { string.Empty };

            char[] SignEquation = { '+', '-', '*', '/'};

            for (var i = 0; i < input.Length; i++)
            {
                words[words.Count-1] += input[i];

                Boolean isLetter1 = false;
                Boolean isLetter2 = false;
              
                if (
                        i + 1 < input.Length 
                   )
                {
                    char input1 = input[i];
                    char input2 = input[i + 1];

                    if (char.IsLetter(input1) || SignEquation.Contains(input1))
                    {
                        isLetter1 = true;
                    }

                    if (char.IsLetter(input2) || SignEquation.Contains(input2))
                    {
                        isLetter2 = true;
                    }

                    if (isLetter1 != isLetter2)
                        words.Add(string.Empty);
                }
            }
            return words;
        }

        public static IEnumerable<int> GetAllNumbersInRange(string input)
        {
            var words = new List<string> { string.Empty };

            char[] SignEquation = { '-' };

            String[] tokens = SplitAlpha(input).ToArray();
            string HoldSign = "";
            List<int> IntList = new List<int>();

            foreach (string item in tokens)
            {
                if (StringHelper.IsNumeric(item))
                {
                    if (HoldSign == "")
                    {
                        IntList.Add(int.Parse(item));
                    }
                    else
                    {
                        if (IntList.Count() > 0)
                        {
                            int lastItem =  IntList.LastOrDefault();

                            while (lastItem < int.Parse(item))
                            {
                                lastItem = lastItem + 1;

                                IntList.Add(lastItem);
                            }
                        }
                    }
                }
                else
                {
                    HoldSign = item;
                }
            }

            return IntList;
        }


    }
}