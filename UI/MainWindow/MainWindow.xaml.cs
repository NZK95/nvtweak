using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void ExtractOptionsFromValue_Click(object sender, RoutedEventArgs e)
        {
            DwordNameTextBox.Visibility = Visibility.Visible;
            DwordValueTextBox.Visibility = Visibility.Visible;
            OptionsTextBox.Visibility = Visibility.Visible;
            ShowOptionsUsed.Visibility = Visibility.Visible;
        }

        private void DwordNameTextBox_LostFocus(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(DwordNameTextBox.Text))
            {
                DwordNameTextBox.Text = "Name";
                DwordNameTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));
            }
        }

        private void DwordNameTextBox_GotFocus(object sender, RoutedEventArgs args)
        {
            if (DwordNameTextBox.Text == "Name")
            {
                DwordNameTextBox.Text = string.Empty;
                DwordNameTextBox.Foreground = Brushes.White;
            }
        }

        private void DwordValueTextBox_LostFocus(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(DwordValueTextBox.Text))
            {
                DwordValueTextBox.Text = "Value";
                DwordValueTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#444444"));
            }
        }

        private void DwordValueTextBox_GotFocus(object sender, RoutedEventArgs args)
        {
            if (DwordValueTextBox.Text == "Value")
            {
                DwordValueTextBox.Text = string.Empty;
                DwordValueTextBox.Foreground = Brushes.White;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private void SetAllToValueButton_Click(object sender, RoutedEventArgs e)
        {
            var valueToSet = SetAllToValueTextBox.Text;

            if (!DWORDValidator.IsDesiredValueInAcceptedRange(valueToSet.ToLower())) return;

            foreach (var item in DwordTreeView.Items)
            {
                var bitRange = DWORDService.ExtractBitRange(Convert.ToString(item));

                if (!string.IsNullOrEmpty(bitRange))
                    DWORDService.ValuesWithBitRanges[bitRange] = valueToSet;
            }

            CalculateValueButton_Click(null, null);
            MessageBox.Show("All options have been setted to desired value", "Bitmask calculated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DisplayDWORDMaxValue()
        {
            if (DWORDService.GetDwordLineIndex(DWORDService.DWORDName) != -1)
            {
                var binaryMaxValue = BitmaskCalculator.GetMaxValue(DWORDService.DWORDName);
                MaxValueTextBox.Text = ConvertorService.ConvertBinaryToHex(binaryMaxValue);
            }
        }
    }
}
