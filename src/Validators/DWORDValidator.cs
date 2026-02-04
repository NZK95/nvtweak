using System.Text.RegularExpressions;
using System.Windows;

namespace nvtweak
{
    internal class DWORDValidator
    {
        public static bool IsDWORDNameEmpty(string DWORDName)
        {
            if (string.IsNullOrWhiteSpace(DWORDName) || string.IsNullOrWhiteSpace(DWORDName))
            {
                MessageBox.Show("Please enter a valid DWORD name or value before proceeding.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public static bool IsDWORDNameFound(int index)
        {
            if (index == -1)
            {
                if (DoesDWORDExistsInNvlddmkm())
                {
                    MessageBox.Show("The specified DWORD exists only in nvlddmkm.\nOften the value of dword can be 0x0 (Disable) / 0x1 (Enabled), and you can undestand it from the name of DWORD.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }

                MessageBox.Show("The specified DWORD was not found in the documentation.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public static bool IsDwordNameEmpty() =>
          string.IsNullOrWhiteSpace(DWORDService.DWORDName);

        public static bool IsLineFromAnotherDWORD(string line, string baseName) =>
            IsLineADwordDefinition(line) && DWORDService.ExtractDwordDefinitionName(line) != baseName;

        public static bool IsLineAComment(string line) =>
          line.StartsWith('/') || line.StartsWith("/*") || line.StartsWith('*') || line.StartsWith("//") || line.StartsWith("*/") || line.StartsWith(" *") || line.StartsWith(" /");

        public static bool AreThereAtLeastOneOptionSelected() =>
           DWORDService.ValuesWithBitRanges.Count > 0;

        public static bool BelongsToBaseDword(int index, string baseDWORDName) =>
            baseDWORDName == DWORDService.ExtractDwordKeyByOption(index);

        public static bool IsLineADwordDefinition(string line) =>
            line.TrimStart().StartsWith("#define") && Regex.IsMatch(line, @"""[^""]+""");

        public static bool DoesDWORDExistsInNvlddmkm()
        {
            foreach (var line in DWORDService.FileLines)
                if (line.Contains(DWORDService.DWORDName))
                    return true;

            return false;
        }

        public static bool IsLineAnOptionDefinition(string line, string definitionName = "")
        {
            if (!line.TrimStart().StartsWith("#define " + definitionName))
            {
                definitionName = definitionName.Contains("NV_REG_STR") ? definitionName.Replace("NV_REG_STR", "NV_REG")
                    : definitionName.Replace("NV_REG", "NV_REG_STR");

                return line.TrimStart().StartsWith("#define " + definitionName) && Regex.IsMatch(line, @"\b\d+:\d+\b");
            }

            return Regex.IsMatch(line, @"\b\d+:\d+\b");
        }
    }
}
