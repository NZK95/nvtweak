using System.Windows;
using System.Windows.Controls;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        public void LoadOptionsAndSuboptionsInTreeView()
        {
            if (!DWORDValidator.IsDWORDNameEmpty(DWORDService.DWORDName)) return;

            var index = DWORDService.GetDwordLineIndex(DWORDService.DWORDName);

            if (!DWORDValidator.IsDWORDNameFound(index)) return;

            DwordTreeView.Items.Clear();

            var rootItem = new TreeViewItem { Header = DWORDService.FileLines[index].Remove(0, 8), IsExpanded = false };
            DwordTreeView.Items.Add(rootItem);

            var options = DWORDService.ExtractOptions(index);

            if (options.Keys.Count is 0)
            {
                ElaborateCaseWithNoOptionsFound(index);
                MessageBox.Show("No options with bit-fields found for this DWORD.\nYou can check the description and the matched lines in TreeView.", "No Options", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (options.Values.All(x => x.Count == 0))
                ElaborateCaseWithNoSubOptionsFound(options, index);
            else
                ElaborateCaseWithOptions(options);
        }


        private void ElaborateCaseWithNoOptionsFound(int index)
        {
            var prefix = "#define " + DWORDService.ExtractDwordDefinitionName(DWORDService.FileLines[index]) + "_";
            HashSet<string> printed = new();

            for (int i = index; i < DWORDService.FileLines.Length; i++)
            {
                var line = DWORDService.FileLines[i];

                if (line.StartsWith(prefix))
                {
                    if (line.IndexOf('/') != -1)
                        line = line.Substring(0, line.IndexOf('/')).Trim();

                    if (printed.Add(line))
                    {
                        line = line.Remove(0, 8);
                        DwordTreeView.Items.Add(line);
                    }
                }
            }
        }

        private void ElaborateCaseWithNoSubOptionsFound(Dictionary<string, List<string>> options, int index)
        {
            SetAllToValueButton.Visibility = Visibility.Visible;
            SetAllToValueTextBox.Visibility = Visibility.Visible;

            ElaborateCaseWithNoOptionsFound(index);
        }

        private void ElaborateCaseWithOptions(Dictionary<string, List<string>> options)
        {
            foreach (var option in options)
            {
                var treeViewItemOptionKey = new TreeViewItem { Header = option.Key.Remove(0, 8).Trim(), IsExpanded = false };

                foreach (var subOption in option.Value)
                {
                    var subCheckBoxInOptionKey = new CheckBox { Content = subOption.Trim(), Tag = option.Key };

                    subCheckBoxInOptionKey.Checked += (s, e) =>
                    {
                        string bitRange = DWORDService.ExtractBitRange(Convert.ToString(subCheckBoxInOptionKey.Tag));
                        string subOptionValue = DWORDService.ExtractSubOptionValue(Convert.ToString(subCheckBoxInOptionKey.Content));
                        DWORDService.ValuesWithBitRanges[bitRange] = subOptionValue;

                        foreach (TreeViewItem siblingItem in treeViewItemOptionKey.Items)
                        {
                            if (siblingItem.Header is CheckBox otherCheckBox && otherCheckBox != subCheckBoxInOptionKey)
                                otherCheckBox.IsChecked = false;
                        }
                    };

                    var subItem = new TreeViewItem { Header = subCheckBoxInOptionKey, IsExpanded = false };
                    treeViewItemOptionKey.Items.Add(subItem);
                }

                DwordTreeView.Items.Add(treeViewItemOptionKey);
            }
        }
    }
}
