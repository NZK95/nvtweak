using System.IO;

namespace nvtweak
{
    internal static class FileService
    {
        public const string REG_FILE_TEMPLATE = @"Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0000]";
        public const string DWORD_DEFINITION_TEMPLATE = "DWORD_DEFINITION_NAME";
        public static readonly string EXPORTED_DWORDS_FILE_TEMPLATE = $"{"DWORD_DEFINITION_NAME".PadRight(100)} DWORD_KEY_NAME\n\n";

        public static void SaveRegistryFile(string path, string valueAssignment)
        {
            if (FileValidator.IsPossibleToWriteRegistryFile(path))
            {
                File.AppendAllText(path, valueAssignment);
            }
            else
            {
                var text = REG_FILE_TEMPLATE + "\n" + valueAssignment;
                File.AppendAllText(path, text);
            }
        }

        public static void SaveExportedStringsFile(string path, string pattern)
        {
            if (File.Exists(path) && File.ReadAllLines(path).Length > 0 && File.ReadAllLines(path)[0].StartsWith(DWORD_DEFINITION_TEMPLATE))
                File.AppendAllText(path, "\n");
            else
                File.AppendAllText(path, EXPORTED_DWORDS_FILE_TEMPLATE);

            var printed = new HashSet<string>();

            for (int i = 0; i < DWORDService.FileLines.Length; i++)
            {
                var line = DWORDService.FileLines[i];

                if (DWORDService.IsLineADwordDefinition(line) && line.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    var dwordDefinitionName = DWORDService.ExtractDwordDefinitionName(line)?.PadRight(100) ?? "NO_DATA".PadRight(100);
                    var dwordKeyName = DWORDService.ExtractDwordKeyName(line);
                    var toInsert = $"{dwordDefinitionName} \"{dwordKeyName}\"\n";

                    if (printed.Add(toInsert))
                        File.AppendAllText(path, toInsert);
                }
            }
        }
    }
}
