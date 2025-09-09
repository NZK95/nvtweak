using System.Windows;
using System.IO;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void SaveToRegFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (NVIDIA.IsDwordNameEmpty() || NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName) == -1 ||
               !NVIDIA.AreThereAtLeastOneOptionSelected())
            {
                MessageBox.Show("Error to save to data into registry file. Please ensure the DWORD name is correct and at least one option is selected.", "Saving Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CalculateValueButton_Click(sender, e);

            const string REG_FILE_TEMPLATE = @"Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0000]";
            string pathToSaveFile = @$"C:\Users\User\Desktop\EXPORTED_REGS.reg";

            string dwordKeyName = NVIDIA.ExtractDwordKeyName(NVIDIA.FileLines[NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName)]);
            string dwordValueAssignment = $"\"{dwordKeyName}\"=dword:{HexResultTextBox.Text.Substring(2)}\n";

            if (File.Exists(pathToSaveFile) && File.ReadAllLines(pathToSaveFile).Length > 0 && File.ReadAllLines(pathToSaveFile)[0] == ("Windows Registry Editor Version 5.00"))
                File.AppendAllText(pathToSaveFile, dwordValueAssignment);
            else
            {
                string textToInsert = REG_FILE_TEMPLATE + "\n" + dwordValueAssignment;
                File.AppendAllText(pathToSaveFile, textToInsert);
            }

            MessageBox.Show("The DWORD value has been successfully saved to the registry file.", "Operation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}