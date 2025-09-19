using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void CalculateValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (NVIDIA.IsDwordNameEmpty() || NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName) == -1 ||
               !NVIDIA.AreThereAtLeastOneOptionSelected()) return;

            string completedBitmask = BitmaskCalculator.GetCompletedBitMask();

            BinaryResultTextBox.Text = completedBitmask;
            DecimalResultTextBox.Text = Convert.ToString(Convert.ToUInt64(BinaryResultTextBox.Text, 2));
            HexResultTextBox.Text = "0x" + Convert.ToString(Convert.ToUInt64(DecimalResultTextBox.Text).ToString("X8"));
        }
    }
}
