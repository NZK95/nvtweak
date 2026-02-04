using Microsoft.Win32;
using System.Windows;

namespace nvtweak
{
    internal static class RegistryService
    {
        public const string NVIDIA_REGISTRY_PATH = @"SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0000";

        public  static void WriteToRegistry(string DWORDName, string value)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(NVIDIA_REGISTRY_PATH, writable: true))
                {
                    if (key == null)
                    {
                        MessageBox.Show("The specified registry key could not be found.", "Registry Key Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    key.SetValue(DWORDName, value, RegistryValueKind.DWord);
                    MessageBox.Show($"The value \"{DWORDName}\" has been successfully set to {value}.", "Registry Updated", MessageBoxButton.OK, MessageBoxImage.Information);
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
