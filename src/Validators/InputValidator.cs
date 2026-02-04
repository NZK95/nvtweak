using System.Text.RegularExpressions;
using System.Windows;

namespace nvtweak
{
    internal static class InputValidator
    {
        public static bool IsHex(string input) => Regex.IsMatch(input, @"^(0x|0X)?[0-9A-Fa-f]+$");

        public static bool IsDecimal(string input) => Regex.IsMatch(input, @"^\d+$");

        public static bool IsBinary(string input) => Regex.IsMatch(input, @"^(0b)?[01]+$");

        public static bool IsDesiredValueInAcceptedRange(string value)
        {
            var acceptedValues = ("0x00000001", "0x00000000");

            if (!value.Contains('x') || value == acceptedValues.Item1 || value == acceptedValues.Item2)
            {
                MessageBox.Show("Invalid value", "Enter new value", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
