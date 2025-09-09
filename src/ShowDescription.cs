using System.Windows;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        private void ShowDescriptionButton_Click(object sender, RoutedEventArgs e)
        {
            SearchButton_Click(sender, e);

            if (NVIDIA.IsDwordNameEmpty() || NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName) == -1)
            {
                MessageBox.Show("Invalid data.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var index = NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName);
            var description = NVIDIA.GetDescription(index);

            Description_Textbox.Text = "Bottom Description:\n\n";
            foreach (var lineOfBottomDescription in description.bottomDescriptionLines)
                Description_Textbox.Text += lineOfBottomDescription + "\n";

            Description_Textbox.Text += "\nTop Description:\n\n";
            foreach (var lineOfTopDescription in description.topDescriptionLines)
                Description_Textbox.Text += lineOfTopDescription + "\n";

            MessageBox.Show("Description is printed.", "Operation Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}