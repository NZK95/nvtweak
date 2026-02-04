using System.Windows;
using System.IO;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void SaveToRegFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (DWORDService.IsDwordNameEmpty() || DWORDService.GetDwordLineIndex(DWORDService.DWORDName) == -1 || !DWORDService.AreThereAtLeastOneOptionSelected())
            {
                MessageBox.Show("Error to save to data into registry file. Please ensure the DWORD name is correct and at least one option is selected.", "Saving Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CalculateValueButton_Click(sender, e);
            
            var path = AppContext.BaseDirectory + "EXPORTED_REGS.reg";
            var key = DWORDService.ExtractDwordKeyName(DWORDService.FileLines[DWORDService.GetDwordLineIndex(DWORDService.DWORDName)]);
            var valueAssignment = $"\"{key}\"=dword:{HexResultTextBox.Text.Substring(2)}\n";

            FileService.SaveRegistryFile(path, valueAssignment);
            MessageBox.Show("The DWORD value has been successfully saved to the registry file.", "Operation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}