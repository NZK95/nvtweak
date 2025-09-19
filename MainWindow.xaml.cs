using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

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

        private void SetAllToValueButton_Click(object sender, RoutedEventArgs e)
        {
            var valueToSet = SetAllToValueTextBox.Text;

            if (!valueToSet.ToLower().Contains('x') || !valueToSet.ToLower().Equals("0x00000000") && !valueToSet.ToLower().Equals("0x00000001"))
            {
                MessageBox.Show("Invalid value", "Enter new value", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var item in DwordTreeView.Items)
            {
                var bitRange = NVIDIA.ExtractBitRange(Convert.ToString(item));
                if (!string.IsNullOrEmpty(bitRange))
                    NVIDIA.ValuesWithBitRanges[bitRange] = valueToSet;
            }

            CalculateValueButton_Click(null, null);
            MessageBox.Show("Setted all options to value", "Bitmask calculated", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowMaxValue()
        {
            var index = NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName);

            if (index != -1)
            {
                var binaryMaxValue = NVIDIA.GetMaxValue(NVIDIA.DWORDName);
                MaxValueTextBox.Text = BitmaskCalculator.ConvertBinaryToHex(binaryMaxValue);
            }
        }
    }
}
