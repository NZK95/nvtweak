using System.IO;

namespace nvtweak
{
    internal static class FileValidator
    {
        public static bool IsPossibleToWriteRegistryFile(string path)
        {
            return File.Exists(path) &&
                File.ReadAllLines(path).Length > 0 &&
                File.ReadAllLines(path)[0] == ("Windows Registry Editor Version 5.00");
        }
    }
}
