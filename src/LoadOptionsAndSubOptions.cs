using System.Windows;
using System.Windows.Controls;

namespace nvtweak
{
    public partial class MainWindow : Window
    {
        public void LoadOptionsAndSuboptionsInTreeView()
        {
            if (NVIDIA.IsDwordNameEmpty())
            {
                MessageBox.Show("Please enter a valid DWORD name before proceeding.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var index = NVIDIA.GetDwordLineIndex(NVIDIA.DWORDName);

            if (index == -1)
            {
                for (int i = 0; i < NVIDIA.FileLines.Length; ++i)
                {
                    var line = NVIDIA.FileLines[i];

                    if (line.Contains(NVIDIA.DWORDName))
                    {
                        MessageBox.Show("The specified DWORD exists only in nvlddmkm.\nOften the value of dword can be 0x0 (Disable) / 0x1 (Enabled), and you can undestand it from the name of DWORD.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }

                MessageBox.Show("The specified DWORD was not found in the documentation.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DwordTreeView.Items.Clear();

            var rootItem = new TreeViewItem { Header = NVIDIA.FileLines[index].Remove(0, 8), IsExpanded = false };
            DwordTreeView.Items.Add(rootItem);

            var options = NVIDIA.ExtractOptions(index);

            if (options.Keys.Count == 0)
            {
                ElaborateCaseWithNoOptionsFound(index);
                MessageBox.Show("No options with bit-fields found for this DWORD.\nYou can check the description and the matched lines in TreeView.", "No Options", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                ElaborateCaseWithOptions(options);
        }


        private void ElaborateCaseWithNoOptionsFound(int index)
        {
            string prefix = "#define " + NVIDIA.ExtractDwordDefinitionName(NVIDIA.FileLines[index]) + "_";
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
