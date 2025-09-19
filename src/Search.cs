using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            NVIDIA.ValuesWithBitRanges.Clear();

            BinaryResultTextBox.Text = BitmaskCalculator.BinaryResultDefaultValue;
            HexResultTextBox.Text = BitmaskCalculator.HexResultkDefaultValue;
            DecimalResultTextBox.Text = BitmaskCalculator.DecimalResultDefaultValue;
            Description_Textbox.Text = string.Empty;
            NVIDIA.DWORDName = SearchBar.Text;

            ShowMaxValue();
            LoadOptionsAndSuboptionsInTreeView();
        }
    }
}