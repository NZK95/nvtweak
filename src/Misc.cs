using System;
using System.Windows;
using System.Xml.Linq;

namespace nvtweak
{
    internal class Misc
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
                if (NVIDIA.DoesDWORDExistsInNvlddmkm())
                {
                    MessageBox.Show("The specified DWORD exists only in nvlddmkm.\nOften the value of dword can be 0x0 (Disable) / 0x1 (Enabled), and you can undestand it from the name of DWORD.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }

                MessageBox.Show("The specified DWORD was not found in the documentation.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public static bool IsDesiredValueInAcceptedRange(string value)
        {
            var acceptedValues = ("0x00000001", "0x00000000");


            if (!value.Contains('x') || value == acceptedValues.Item1 || value == acceptedValues.Item2)
            {
                MessageBox.Show("Invalid value", "Enter new value", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
