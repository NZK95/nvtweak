using System.Windows;
using System.IO;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void ExportDwordsFromDocumentationButton_Click(object sender, RoutedEventArgs e)
        {
            var pattern = SearchBar.Text;
            var path = AppContext.BaseDirectory + "NVIDIA-EXPORTED-STRING.txt";

            FileService.SaveExportedStringsFile(pattern, path);
            MessageBox.Show($"Successfully exported dword with \"{(string.IsNullOrEmpty(pattern) ? "Empty" : pattern)}\" pattern.", "Operation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
