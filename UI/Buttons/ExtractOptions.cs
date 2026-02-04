using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void ShowOptionsUsed_Click(object sender, RoutedEventArgs e)
        {
            OptionsTextBox.Text = null;

            var name = (DwordNameTextBox.Text == "Name") ? string.Empty : DwordNameTextBox.Text;
            var value = (DwordValueTextBox.Text == "Value") ? string.Empty : DwordValueTextBox.Text;

            if (!DWORDValidator.IsDWORDNameEmpty(name)) return;

            var index = DWORDService.GetDwordLineIndex(name);

            if (!DWORDValidator.IsDWORDNameFound(index)) return;

            var options = DWORDService.ExtractOptions(DWORDService.GetDwordLineIndex(name));

            if (options.Keys.Count is 0)
            {
                ElaborateCaseWhenKeysCountIsZero(index, value);
                return;
            }

            if (!IsInputValueValid(value,name)) return;

            var optionsUsed = new List<string>();
            var bitRanges = ExtractBitRangesFromListOfOptions(options.Keys.ToList());
            var hexValues = ExtractHexValuesFromBitmaskAndBitRanges(bitRanges);
            var countForHexValues = 0;

            foreach (var key in options.Keys)
            {
                foreach (var subOption in options[key])
                {
                    if (subOption.Contains(hexValues[countForHexValues], StringComparison.OrdinalIgnoreCase))
                    {
                        var bitRange = DWORDService.ExtractBitRange(key);

                        var line = key[8..].Replace(bitRange, string.Empty).Trim();
                        line += $" ({bitRange}) - {subOption.Trim()}";

                        optionsUsed.Add(line);
                    }
                }

                ++countForHexValues;
            }

            OptionsTextBox.Text = string.Join("\n\n", optionsUsed);
            MessageBox.Show("All options used are printed", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ElaborateCaseWhenKeysCountIsZero(int index, string value)
        {
            var dwordDefinitionName = DWORDService.ExtractDwordDefinitionName(DWORDService.FileLines[index]);

            for (int i = index; i < DWORDService.FileLines.Length; i++)
            {
                var line = DWORDService.FileLines[i];
                var expression = $"#define {dwordDefinitionName}";

                if (line.StartsWith(expression) && line.Contains(value))
                {
                    OptionsTextBox.Text = line;
                    break;
                }
            }
        }

        private List<string> ExtractHexValuesFromBitmaskAndBitRanges(List<string> bitRanges)
        {
            var userValue = DwordValueTextBox.Text;
            var binaryValue = ConvertorService.ConvertToBinaryAnySystem(userValue);
            binaryValue = new string('0', BitmaskCalculator.BINARY_DEFAULT_VALUE.Length - binaryValue.Length) + binaryValue;

            var hexValues = new List<string>();

            foreach (var bitRange in bitRanges)
            {
                var startIndex = Convert.ToInt32(bitRange.Split(':')[1]);
                var endIndex = Convert.ToInt32(bitRange.Split(':')[0]);
                var chars = new List<char>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    var charToInsert = binaryValue[binaryValue.Length - 1 - i];
                    chars.Add(charToInsert);
                }

                chars.Reverse();

                var resultInBinary = ConvertorService.ConvertBinaryToHex(new string(chars.ToArray()));
                hexValues.Add(resultInBinary);
            }

            return hexValues;
        }

        private bool IsInputValueValid(string value, string DWORDName)
        {
            var decimalUserValue = Convert.ToUInt64(ConvertorService.ConvertToBinaryAnySystem(value), 2);
            var decimalMaxValue = Convert.ToUInt64(ConvertorService.ConvertToBinaryAnySystem(BitmaskCalculator.GetMaxValue(DWORDName)), 2);

            if (decimalUserValue > decimalMaxValue)
            {
                MessageBox.Show("Invalid value.", "Invalid input value", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private List<string> ExtractBitRangesFromListOfOptions(List<string> options) =>
           options.Select(x => DWORDService.ExtractBitRange(x)).ToList();
    }
}
