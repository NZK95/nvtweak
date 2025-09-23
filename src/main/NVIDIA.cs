using System.IO;
using System.Text.RegularExpressions;

namespace nvtweak
{
    internal class NVIDIA
    {
        public static string FilePath { get; private set; } = @"C:\Users\User\Desktop\NEWERA\NVIDIA-DOC.txt";
        public static string[] FileLines { get; private set; } = File.ReadAllLines(FilePath);
        public static Dictionary<string, int> DwordLineIndexCache { get; private set; } = new();
        public static Dictionary<string, string> ValuesWithBitRanges { get; private set; } = new();
        public static string DWORDName { get; set; }

        public static Dictionary<string, List<string>> ExtractOptions(int startIndex)
        {
            if (startIndex == -1) return null;

            var options = new Dictionary<string, List<string>>();
            string DWORDDefinitionName = ExtractDwordDefinitionName(FileLines[startIndex]);
            string DWORDKeyName = ExtractDwordKeyName(FileLines[startIndex]);

            for (int i = startIndex; i < FileLines.Length; i++)
            {
                var line = FileLines[i];

                if (!IsLineAnOptionDefinition(line, DWORDDefinitionName) || !BelongsToBaseDword(i, DWORDKeyName))
                    continue;

                string optionKey = CleanKeyLineFromComments(line);
                var valueLines = new List<string>();

                for (int j = i + 1; ; ++j)
                {
                    var secondLine = FileLines[j];

                    if (IsLineAnOptionDefinition(secondLine, DWORDDefinitionName) || IsLineFromAnotherDWORD(secondLine, DWORDDefinitionName))
                        break;

                    if (string.IsNullOrWhiteSpace(secondLine) || IsLineAComment(secondLine))
                        continue;

                    string cleanValue = CleanValueLineFromCommentsAndBasePrefix(secondLine, DWORDDefinitionName);

                    if (!string.IsNullOrWhiteSpace(cleanValue))
                        valueLines.Add(cleanValue);
                }

                options[optionKey] = valueLines;
            }

            options = RemoveKeysWithFewestOptions(options);
            return options;
        }

        public static Dictionary<string, List<string>> RemoveKeysWithFewestOptions(Dictionary<string, List<string>> options)
        {
            var keysSnapshot = options.Keys.ToList();
            var keysToRemove = new HashSet<string>();

            foreach (var key in keysSnapshot)
            {
                if (keysToRemove.Contains(key))
                    continue;

                var bitRange = ExtractBitRange(key);
                var matchingKeys = keysSnapshot.Where(k => ExtractBitRange(k).Equals(bitRange)).ToList();

                if (matchingKeys.Count > 1)
                {
                    string keyToKeep = matchingKeys.OrderByDescending(k => options[k].Count).First();

                    foreach (var k in matchingKeys)
                    {
                        if (k != keyToKeep)
                            keysToRemove.Add(k);
                    }
                }
            }

            foreach (var key in keysToRemove)
                options.Remove(key);

            return options;
        }

        public static int GetDwordLineIndex(string dwordName)
        {
            if (DwordLineIndexCache.TryGetValue(dwordName.ToLower(), out int cachedIndex))
                return cachedIndex;

            for (int i = 0; i < FileLines.Length; i++)
            {
                if (IsLineADwordDefinition(FileLines[i]) && ExtractDwordKeyName(FileLines[i]).Equals(dwordName, StringComparison.OrdinalIgnoreCase))
                {
                    DwordLineIndexCache[dwordName.ToLower()] = i;
                    return i;
                }
            }

            return -1;
        }

        public static (List<string> bottomDescriptionLines, List<string> topDescriptionLines) GetDescription(int indexOfLine)
        {
            var currentDwordBase = $"#define {ExtractDwordDefinitionName(FileLines[indexOfLine])}";

            bool IsRelevantDefinition(string line)
            {
                if (!line.StartsWith(currentDwordBase))
                {
                    line = line.Contains("NV_REG") ? line.Replace("NV_REG", "NV_REG_STR") : line.Replace("NV_REG_STR", "NV_REG");
                    return line.StartsWith(currentDwordBase);
                }

                return true;
            }

            List<string> ExtractDescriptionLines(int startIndex, int direction)
            {
                var descriptionLines = new List<string>();

                for (int i = startIndex; i >= 0 && i < FileLines.Length; i += direction)
                {
                    var line = FileLines[i];

                    if (line.StartsWith("#define") && !IsRelevantDefinition(line))
                        break;

                    descriptionLines.Add(line);
                }

                return descriptionLines;
            }

            var bottom = ExtractDescriptionLines(indexOfLine + 1, +1);
            var top = ExtractDescriptionLines(indexOfLine - 1, -1);

            return (bottom, top);
        }

        public static string ExtractDwordKeyName(string line)
        {
            // Example:
            // Input: #define NV_REG_RM_TWIN_PEAKS_SUPPORT             "RMTwinPeaksSupport"
            // Output: RMTwinPeaksSupport

            var match = Regex.Match(line, @"""([^""]+)""");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public static string? ExtractDwordDefinitionName(string line)
        {
            // Starts from 7 to skip "#define".
            // Example:
            // Input: #define NV_REG_RM_TWIN_PEAKS_SUPPORT             "RMTwinPeaksSupport"
            // Output: NV_REG_RM_TWIN_PEAKS_SUPPORT

            if (line.IndexOf('\"') == -1) return null;
            return line[7..line.IndexOf('\"')].Trim();
        }

        public static string ExtractBitRange(string line)
        {
            // Example: #define NV_REG_STR_RM_CLK_VF_OVERRIDE_MONOTONICITY_ENABLE                    1:0
            // Output: 1:0

            var match = Regex.Match(line, @"\b\d+:\d+\b$");
            return match.Success ? match.Value : string.Empty;
        }

        public static string ExtractDwordKeyByOption(int indexOfLine)
        {
            for (int i = indexOfLine; i > 0; --i)
            {
                if (IsLineADwordDefinition(FileLines[i]))
                    return ExtractDwordKeyName(FileLines[i]);
            }

            return string.Empty;

        }

        public static string ExtractSubOptionValue(string definition)
        {
            definition = (definition.StartsWith("#define")) ? definition[8..].Trim() : definition.Trim();

            int spaceIndex = definition.IndexOf(' ');
            definition = (spaceIndex >= 0) ? definition[(spaceIndex + 1)..].Trim() : definition;

            return definition.Trim().Trim('(', ')');
        }

        public static bool DoesDWORDExistsInNvlddmkm()
        {
            foreach (var line in FileLines)
                if (line.Contains(DWORDName))
                    return true;

            return false;
        }

        public static bool IsLineADwordDefinition(string line) =>
            line.TrimStart().StartsWith("#define") && Regex.IsMatch(line, @"""[^""]+""");

        private static bool IsLineAnOptionDefinition(string line, string definitionName = "")
        {
            var expression = "#define " + definitionName;
            line = line.TrimStart();

            if (!line.StartsWith(expression))
            {
                definitionName = (definitionName.Contains("NV_REG_STR")) ? definitionName.Replace("NV_REG_STR", "NV_REG")
                    : definitionName.Replace("NV_REG", "NV_REG_STR");

                return line.StartsWith(expression) && Regex.IsMatch(line, @"\b\d+:\d+\b");
            }

            return Regex.IsMatch(line, @"\b\d+:\d+\b");
        }

        public static bool IsDwordNameEmpty() =>
            string.IsNullOrWhiteSpace(DWORDName);

        public static bool IsLineFromAnotherDWORD(string line, string baseName) =>
            IsLineADwordDefinition(line) && ExtractDwordDefinitionName(line) != baseName;

        public static bool IsLineAComment(string line) =>
          line.StartsWith('/') || line.StartsWith("/*") || line.StartsWith('*') || line.StartsWith("//") || line.StartsWith("*/") || line.StartsWith(" *") || line.StartsWith(" /");

        public static bool AreThereAtLeastOneOptionSelected() =>
           ValuesWithBitRanges.Count > 0;

        private static bool BelongsToBaseDword(int index, string baseDWORDName) => baseDWORDName == ExtractDwordKeyByOption(index);

        private static string CleanKeyLineFromComments(string line)
        {
            int commentIndex = line.IndexOf('/');

            if (commentIndex != -1)
                line = line.Substring(0, commentIndex);

            return line.Trim();
        }

        private static string CleanValueLineFromCommentsAndBasePrefix(string line, string baseName)
        {
            line = CleanKeyLineFromComments(line);

            return (line.Length) < 8 ? string.Empty
                : line.Substring(8).Replace(baseName, " ").Trim();
        }
       
    }
}
