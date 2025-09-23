using System.Windows;
using System.Windows.Controls;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        public void LoadOptionsAndSuboptionsInTreeView()
        {
            if (!Misc.IsDWORDNameEmpty(NVIDIA.DWORDName)) return;

            var index = NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName);

            if (!Misc.IsDWORDNameFound(index)) return;

            DwordTreeView.Items.Clear();

            var rootItem = new TreeViewItem { Header = NVIDIA.FileLines[index].Remove(0, 8), IsExpanded = false };
            DwordTreeView.Items.Add(rootItem);

            var options = NVIDIA.ExtractOptions(index);

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
            var prefix = "#define " + NVIDIA.ExtractDwordDefinitionName(NVIDIA.FileLines[index]) + "_";
            HashSet<string> printed = new();

            for (int i = index; i < NVIDIA.FileLines.Length; i++)
            {
                var line = NVIDIA.FileLines[i];

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
                        string bitRange = NVIDIA.ExtractBitRange(Convert.ToString(subCheckBoxInOptionKey.Tag));
                        string subOptionValue = NVIDIA.ExtractSubOptionValue(Convert.ToString(subCheckBoxInOptionKey.Content));
                        NVIDIA.ValuesWithBitRanges[bitRange] = subOptionValue;

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
