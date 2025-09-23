using System.Text.RegularExpressions;

namespace nvtweak
{
    internal class BitmaskCalculator
    {
        public static string BinaryResultDefaultValue { get; private set; } = "00000000000000000000000000000000";
        public static string DecimalResultDefaultValue { get; private set; } = "0";
        public static string HexResultkDefaultValue { get; private set; } = "0x00000000";

        public static string GetCompletedBitMask()
        {
            var bitRanges = NVIDIA.ValuesWithBitRanges.Keys.ToList();
            var values = NVIDIA.ValuesWithBitRanges.Values.ToList();
            var result = CalculateBitMask(NVIDIA.ValuesWithBitRanges.Count, values, bitRanges);
           
            return string.Join("", result.Reverse());
        }

        public static char[] CalculateBitMask(int collectionCount, List<string> values, List<string> bitRanges)
        {
            char[] chars = BinaryResultDefaultValue.ToCharArray();

            for (int i = 0; i < collectionCount; i++)
            {
                if (values[i] == null) continue;

                string binaryValue = ConvertToBinary(values[i]);
                binaryValue = new string('0', BinaryResultDefaultValue.Length - binaryValue.Length) + binaryValue;

                int endIndex = Convert.ToInt32(bitRanges[i].Split(':')[0]);
                int startIndex = Convert.ToInt32(bitRanges[i].Split(':')[1]);

                for (int j = startIndex, count = 0; j <= endIndex; j++, count++)
                {
                    char charToInsert = binaryValue[binaryValue.Length - count - 1];
                    chars[j] = charToInsert;
                }
            }

            return chars;
        }

        public static string GetMaxValue(string dwordName)
        {
            var index = NVIDIA.GetDwordLineIndex(dwordName);
            var bitRanges = NVIDIA.ExtractOptions(index).Keys.Select(x => NVIDIA.ExtractBitRange(x)).ToList();
            var values = NVIDIA.ExtractOptions(index).Values.Select(x => (x.Count >= 1) ? NVIDIA.ExtractSubOptionValue(x[x.Count - 1]) : null).ToList();

            var result = CalculateBitMask(bitRanges.Count, values, bitRanges);

            return string.Join("", result.Reverse());
        }

        public static string ConvertToBinary(string input)
        {
            ulong number;

            if (IsBinary(input))
                return input.StartsWith("0b") ? input.Substring(2) : input;

            else if (IsDecimal(input))
                number = ulong.Parse(input);

            else if (IsHex(input))
            {
                string hex = input.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? input.Substring(2) : input;
                number = Convert.ToUInt64(hex, 16);
            }

            else throw new FormatException("Unknown numeric system.");

            return Convert.ToString((long)number, 2);
        }

        public static string ConvertBinaryToHex(string binary)
        {
            var decimalValue = Convert.ToUInt64(binary, 2);
            string hexValue = decimalValue.ToString("X");
            return "0x" + decimalValue.ToString("X8");
        }

        private static bool IsHex(string input) => Regex.IsMatch(input, @"^(0x|0X)?[0-9A-Fa-f]+$");

        private static bool IsDecimal(string input) => Regex.IsMatch(input, @"^\d+$");

        private static bool IsBinary(string input) => Regex.IsMatch(input, @"^(0b)?[01]+$");
    }
}
