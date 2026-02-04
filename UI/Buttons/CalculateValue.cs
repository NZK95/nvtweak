using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void CalculateValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (DWORDService.IsDwordNameEmpty() || DWORDService.GetDwordLineIndex(DWORDService.DWORDName) == -1 ||
               !DWORDService.AreThereAtLeastOneOptionSelected()) return;

            BinaryResultTextBox.Text = BitmaskCalculator.GetCompletedBitMask();
            DecimalResultTextBox.Text = Convert.ToString(Convert.ToUInt64(BinaryResultTextBox.Text, 2));
            HexResultTextBox.Text = "0x" + Convert.ToString(Convert.ToUInt64(DecimalResultTextBox.Text).ToString("X8"));
        }
    }
}
