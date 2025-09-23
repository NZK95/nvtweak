using System.Windows;
using System.IO;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void ExportDwordsFromDocumentationButton_Click(object sender, RoutedEventArgs e)
        {
            var printed = new HashSet<string>();
            var patternToMatch = SearchBar.Text;
            var pathToFile = AppContext.BaseDirectory + "NVIDIA-EXPORTED-STRING.txt";
            string EXPORTED_DWORDS_FILE_TEMPLATE = $"{"DWORD_DEFINITION_NAME".PadRight(100)} DWORD_KEY_NAME\n\n";

            if (File.Exists(pathToFile) && File.ReadAllLines(pathToFile).Length > 0 && File.ReadAllLines(pathToFile)[0].StartsWith("DWORD_DEFINITION_NAME"))
                File.AppendAllText(pathToFile, "\n");
            else
                File.AppendAllText(pathToFile, EXPORTED_DWORDS_FILE_TEMPLATE);

            for (int i = 0; i < NVIDIA.FileLines.Length; i++)
            {
                var line = NVIDIA.FileLines[i];

                if (NVIDIA.IsLineADwordDefinition(line) && line.Contains(patternToMatch, StringComparison.OrdinalIgnoreCase))
                {
                    string dwordDefinitionName = NVIDIA.ExtractDwordDefinitionName(line)?.PadRight(100) ?? "NO_DATA".PadRight(100);
                    string dwordKeyName = NVIDIA.ExtractDwordKeyName(line);
                    string toInsert = $"{dwordDefinitionName} \"{dwordKeyName}\"\n";

                    if (printed.Add(toInsert))
                        File.AppendAllText(pathToFile, toInsert);
                }
            }

            MessageBox.Show($"Successfully exported dword with \"{(string.IsNullOrEmpty(patternToMatch) ? "Empty" : patternToMatch)}\" pattern.", "Operation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
