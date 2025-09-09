using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void ShowOptionsUsed_Click(object sender, RoutedEventArgs e)
        {
            var name = (DwordNameTextBox.Text == "Name") ? string.Empty : DwordNameTextBox.Text;
            var value = (DwordValueTextBox.Text == "Value") ? string.Empty : DwordValueTextBox.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("Please enter a valid DWORD name or value before proceeding.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var index = NVIDIA.GetDwordLineIndex(name);

            if (index == -1)
            {
                for (int i = 0; i < NVIDIA.FileLines.Length; ++i)
                {
                    var line = NVIDIA.FileLines[i];

                    if (line.Contains(name))
                    {
                        MessageBox.Show("The specified DWORD exists only in nvlddmkm.\nOften the value of dword can be 0x0 (Disable) / 0x1 (Enabled), and you can undestand it from the name of DWORD.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                MessageBox.Show("The specified DWORD was not found in the documentation.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var decimalUserValue = Convert.ToInt32(BitmaskCalculator.ConvertToBinary(value), 2);
            var decimalMaxValue = Convert.ToInt32(BitmaskCalculator.ConvertToBinary(string.IsNullOrWhiteSpace(MaxValueTextBox.Text) ? NVIDIA.GetMaxValue(name) : MaxValueTextBox.Text), 2);

            if (decimalUserValue > decimalMaxValue)
            {
                MessageBox.Show("Invalid value.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var optionsUsed = new List<string>();
            var options = NVIDIA.ExtractOptions(NVIDIA.GetDwordLineIndex(name));

            var bitRanges = GetBitRangesFromListOfOptions(options.Keys.ToList());
            var hexValues = GetHexValuesFromBitmaskAndBitRanges(bitRanges);

            var countForHexValues = 0;
            foreach (var key in options.Keys)
            {
                foreach (var subOption in options[key])
                {
                    if (subOption.Contains(hexValues[countForHexValues], StringComparison.OrdinalIgnoreCase))
                    {
                        var bitRange = NVIDIA.ExtractBitRange(key);
                        var line = key[8..].Replace(bitRange, string.Empty).Trim();
                        line += $" ({bitRange}) - {subOption[1..].Trim()}";
                        optionsUsed.Add(line);
                    }
                }
                ++countForHexValues;
            }

            var result = string.Join("\n\n", optionsUsed);
            OptionsTextBox.Text = result;
            MessageBox.Show("All options used are printed", "", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private List<string> GetBitRangesFromListOfOptions(List<string> options) =>
            options.Select(x => NVIDIA.ExtractBitRange(x)).ToList();

        private List<string> GetHexValuesFromBitmaskAndBitRanges(List<string> bitRanges)
        {
            var userValue = DwordValueTextBox.Text;
            var binaryValue = BitmaskCalculator.ConvertToBinary(userValue);
            binaryValue = new string('0', BitmaskCalculator.BinaryResultDefaultValue.Length - binaryValue.Length) + binaryValue;

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

                var resultInBinary = BitmaskCalculator.ConvertBinaryToHex(new string(chars.ToArray()));
                hexValues.Add(resultInBinary);
            }

            return hexValues;
        }
    }
}
