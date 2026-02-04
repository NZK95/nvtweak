using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace nvtweak
{
    internal class DWORDService
    {
        public static string FilePath { get; private set; }
        public static string DWORDName { get; set; }
        public static string[] FileLines { get; private set; }
        public static Dictionary<string, int> DwordLineIndexCache { get; private set; }
        public static Dictionary<string, string> ValuesWithBitRanges { get; private set; }

        static DWORDService()
        {
            InitializeFilePath();
            LoadDocumentation();
            InitializeCaches();
        }

        public static Dictionary<string, List<string>> ExtractOptions(int startIndex)
        {
            if (startIndex == -1)
                return null;

            var options = new Dictionary<string, List<string>>();
            var definitionName = ExtractDwordDefinitionName(FileLines[startIndex]);
            var keyName = ExtractDwordKeyName(FileLines[startIndex]);

            for (int i = startIndex; i < FileLines.Length; i++)
            {
                if (!IsValidOptionLine(i, definitionName, keyName))
                    continue;

                string optionKey = CleanKeyLineFromComments(FileLines[i]);
                var valueLines = ExtractOptionValues(i, definitionName);

                options[optionKey] = valueLines;
            }

            return RemoveKeysWithFewestOptions(options);
        }

        public static int GetDwordLineIndex(string dwordName)
        {
            if (DwordLineIndexCache.TryGetValue(dwordName.ToLower(), out int cachedIndex))
                return cachedIndex;

            for (int i = 0; i < FileLines.Length; i++)
            {
                if (!DWORDValidator.IsLineADwordDefinition(FileLines[i]))
                    continue;

                if (ExtractDwordKeyName(FileLines[i]).Equals(dwordName, StringComparison.OrdinalIgnoreCase))
                {
                    DwordLineIndexCache[dwordName.ToLower()] = i;
                    return i;
                }
            }

            return -1;
        }

        public static (List<string> bottomDescriptionLines, List<string> topDescriptionLines) GetDescription(int indexOfLine)
        {
            string currentDwordBase = BuildDwordBaseDefinition(FileLines[indexOfLine]);

            var bottom = ExtractDescriptionLines(indexOfLine + 1, +1, currentDwordBase);
            var top = ExtractDescriptionLines(indexOfLine - 1, -1, currentDwordBase);

            return (bottom, top);
        }

        public static string ExtractDwordKeyName(string line)
        {
            var match = Regex.Match(line, @"""([^""]+)""");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public static string ExtractDwordDefinitionName(string line)
        {
            int quoteIndex = line.IndexOf('\"');
            if (quoteIndex == -1)
                return null;

            return line[7..quoteIndex].Trim();
        }

        public static string ExtractBitRange(string line)
        {
            var match = Regex.Match(line, @"\b\d+:\d+\b$");
            return match.Success ? match.Value : string.Empty;
        }

        public static string ExtractDwordKeyByOption(int indexOfLine)
        {
            for (int i = indexOfLine; i > 0; --i)
            {
                if (DWORDValidator.IsLineADwordDefinition(FileLines[i]))
                    return ExtractDwordKeyName(FileLines[i]);
            }

            return string.Empty;
        }

        public static string ExtractSubOptionValue(string definition)
        {
            definition = RemoveDefinePrefix(definition);
            definition = RemoveSymbolPrefix(definition);
            return definition.Trim().Trim('(', ')');
        }


        private static void InitializeFilePath()
        {
            FilePath = Path.Combine(AppContext.BaseDirectory, "NVIDIA-DOCUMENTATION.txt");
        }

        private static void LoadDocumentation()
        {
            if (File.Exists(FilePath))
            {
                FileLines = File.ReadAllLines(FilePath);
            }
            else
            {
                const string errorTitle = "Missing file.";
                const string errorMessage = "NVIDIA-DOCUMENTATION.txt not found";
                MessageBox.Show(errorMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
        }

        private static void InitializeCaches()
        {
            DwordLineIndexCache = new Dictionary<string, int>();
            ValuesWithBitRanges = new Dictionary<string, string>();
        }

        private static Dictionary<string, List<string>> RemoveKeysWithFewestOptions(
            Dictionary<string, List<string>> options)
        {
            var keysSnapshot = options.Keys.ToList();
            var keysToRemove = new HashSet<string>();

            foreach (var key in keysSnapshot)
            {
                if (keysToRemove.Contains(key))
                    continue;

                var bitRange = ExtractBitRange(key);
                var matchingKeys = keysSnapshot
                    .Where(k => ExtractBitRange(k).Equals(bitRange))
                    .ToList();

                if (matchingKeys.Count <= 1)
                    continue;

                string keyToKeep = matchingKeys
                    .OrderByDescending(k => options[k].Count)
                    .First();

                foreach (var k in matchingKeys.Where(k => k != keyToKeep))
                {
                    keysToRemove.Add(k);
                }
            }

            foreach (var key in keysToRemove)
            {
                options.Remove(key);
            }

            return options;
        }

        private static bool IsValidOptionLine(int lineIndex, string definitionName, string keyName)
        {
            string line = FileLines[lineIndex];
            return DWORDValidator.IsLineAnOptionDefinition(line, definitionName) &&
                   DWORDValidator.BelongsToBaseDword(lineIndex, keyName);
        }

        private static List<string> ExtractOptionValues(int optionIndex, string definitionName)
        {
            var valueLines = new List<string>();

            for (int j = optionIndex + 1; j < FileLines.Length; j++)
            {
                string line = FileLines[j];

                if (IsOptionBoundary(line, definitionName))
                    break;

                if (string.IsNullOrWhiteSpace(line) || DWORDValidator.IsLineAComment(line))
                    continue;

                string cleanValue = CleanValueLineFromCommentsAndBasePrefix(line, definitionName);

                if (!string.IsNullOrWhiteSpace(cleanValue))
                    valueLines.Add(cleanValue);
            }

            return valueLines;
        }

        private static bool IsOptionBoundary(string line, string definitionName)
        {
            return DWORDValidator.IsLineAnOptionDefinition(line, definitionName) ||
                   DWORDValidator.IsLineFromAnotherDWORD(line, definitionName);
        }

        private static List<string> ExtractDescriptionLines(int startIndex, int direction, string currentDwordBase)
        {
            var descriptionLines = new List<string>();

            for (int i = startIndex; i >= 0 && i < FileLines.Length; i += direction)
            {
                string line = FileLines[i];

                if (line.StartsWith("#define") && !IsRelevantDefinition(line, currentDwordBase))
                    break;

                descriptionLines.Add(line);
            }

            return descriptionLines;
        }

        private static bool IsRelevantDefinition(string line, string currentDwordBase)
        {
            if (line.StartsWith(currentDwordBase))
                return true;

            line = line.Contains("NV_REG")
                ? line.Replace("NV_REG", "NV_REG_STR")
                : line.Replace("NV_REG_STR", "NV_REG");

            return line.StartsWith(currentDwordBase);
        }

        private static string BuildDwordBaseDefinition(string line)
        {
            return $"#define {ExtractDwordDefinitionName(line)}";
        }

        private static string RemoveDefinePrefix(string definition)
        {
            return definition.StartsWith("#define")
                ? definition[8..].Trim()
                : definition.Trim();
        }

        private static string RemoveSymbolPrefix(string definition)
        {
            int spaceIndex = definition.IndexOf(' ');
            return spaceIndex >= 0
                ? definition[(spaceIndex + 1)..].Trim()
                : definition;
        }

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

            return line.Length < 8
                ? string.Empty
                : line.Substring(8).Replace(baseName, " ").Trim();
        }
    }
}