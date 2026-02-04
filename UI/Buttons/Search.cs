using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DWORDService.ValuesWithBitRanges.Clear();
            DWORDService.DWORDName = SearchBar.Text;
            FillTextBoxesWithValues();
            DisplayDWORDMaxValue();
            LoadOptionsAndSuboptionsInTreeView();
        }

        private void FillTextBoxesWithValues()
        {
            BinaryResultTextBox.Text = BitmaskCalculator.BINARY_DEFAULT_VALUE;
            HexResultTextBox.Text = BitmaskCalculator.HEX_DEFAULT_VALUE;
            DecimalResultTextBox.Text = BitmaskCalculator.DECIMAL_DEFAULT_VALUE;
            Description_Textbox.Text = string.Empty;
        }
    }
}