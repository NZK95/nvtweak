using System.Text.RegularExpressions;

namespace nvtweak
{
    internal class BitmaskCalculator
    {
        public const string BINARY_DEFAULT_VALUE = "00000000000000000000000000000000";
        public const string DECIMAL_DEFAULT_VALUE  = "0";
        public const string HEX_DEFAULT_VALUE  = "0x00000000";

        public static string GetCompletedBitMask()
        {
            var bitRanges = DWORDService.ValuesWithBitRanges.Keys.ToList();
            var values = DWORDService.ValuesWithBitRanges.Values.ToList();
            var result = CalculateBitMask(DWORDService.ValuesWithBitRanges.Count, values, bitRanges);
           
            return string.Join("", result.Reverse());
        }

        public static char[] CalculateBitMask(int collectionCount, List<string> values, List<string> bitRanges)
        {
            var chars = BINARY_DEFAULT_VALUE.ToCharArray();

            for (int i = 0; i < collectionCount; i++)
            {
                if (values[i] == null) continue;

                var binaryValue = ConvertorService.ConvertToBinaryAnySystem(values[i]);
                binaryValue = new string('0', BINARY_DEFAULT_VALUE.Length - binaryValue.Length) + binaryValue;

                var endIndex = Convert.ToInt32(bitRanges[i].Split(':')[0]);
                var startIndex = Convert.ToInt32(bitRanges[i].Split(':')[1]);

                for (int j = startIndex, count = 0; j <= endIndex; j++, count++)
                {
                    var charToInsert = binaryValue[binaryValue.Length - count - 1];
                    chars[j] = charToInsert;
                }
            }

            return chars;
        }

        public static string GetMaxValue(string dwordName)
        {
            var index = DWORDService.GetDwordLineIndex(dwordName);
            var bitRanges = DWORDService.ExtractOptions(index).Keys.Select(x => DWORDService.ExtractBitRange(x)).ToList();
            var values = DWORDService.ExtractOptions(index).Values.Select(x => (x.Count >= 1) ? DWORDService.ExtractSubOptionValue(x[x.Count - 1]) : null).ToList();
            var result = CalculateBitMask(bitRanges.Count, values, bitRanges);

            return string.Join("", result.Reverse());
        }
    }
}
