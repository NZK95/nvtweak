namespace nvtweak
{
    internal static class ConvertorService
    {
        public static string ConvertToBinaryAnySystem(string input)
        {
            ulong number;

            if (DWORDValidator.IsBinary(input))
            {
                return input.StartsWith("0b") ? input.Substring(2) : input;
            }
            else if (DWORDValidator.IsDecimal(input))
            {
                number = ulong.Parse(input);
            }
            else if (DWORDValidator.IsHex(input))
            {
                var hex = input.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? input.Substring(2) : input;
                number = Convert.ToUInt64(hex, 16);
            }
            else
            {
                throw new FormatException("Unknown numeric system.");
            }

            return Convert.ToString((long)number, 2);
        }

        public static string ConvertBinaryToHex(string binary)
        {
            var decimalValue = Convert.ToUInt64(binary, 2);
            var hexValue = decimalValue.ToString("X");
            return "0x" + decimalValue.ToString("X8");
        }
    }
}
