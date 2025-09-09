using Microsoft.Win32;
using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void ApplyToRegistryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!NVIDIA.AreThereAtLeastOneOptionSelected())
            {
                MessageBox.Show("Please select at least one option before applying changes.", "No Options Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CalculateValueButton_Click(sender, e);
            const string path = @"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0000";
            string fullDwordName = NVIDIA.ExtractDwordKeyName(NVIDIA.FileLines[NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName)]);
            string valueToSet = DecimalResultTextBox.Text;

            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path, writable: true))
                {
                    if (key == null)
                    {
                        MessageBox.Show("The specified registry key could not be found.", "Registry Key Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    key.SetValue(fullDwordName, valueToSet, RegistryValueKind.DWord);
                    MessageBox.Show($"The value \"{fullDwordName}\" has been successfully set to {HexResultTextBox.Text}.", "Registry Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access denied. Please run the application as an administrator.", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
