using Microsoft.Win32;
using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void ApplyToRegistryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DWORDService.AreThereAtLeastOneOptionSelected())
            {
                MessageBox.Show("Please select at least one option before applying changes.", "No Options Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CalculateValueButton_Click(sender, e);

            var DWORDName = DWORDService.ExtractDwordKeyName(DWORDService.FileLines[DWORDService.GetDwordLineIndex(DWORDService.DWORDName)]);
            var value = DecimalResultTextBox.Text;

            RegistryService.WriteToRegistry(DWORDName, value);
        }
    }
}
