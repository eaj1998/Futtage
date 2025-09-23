using System;
using System.IO;

namespace Futtage.Infrastructure.Configuration
{
    public static class ConfigurationHelper
    {
        public static string ExpandEnvironmentPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            try
            {
                var expanded = Environment.ExpandEnvironmentVariables(path);

                if (expanded.Contains("%"))
                {
                    expanded = expanded.Replace("%LOCALAPPDATA%",
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        StringComparison.OrdinalIgnoreCase);

                    expanded = expanded.Replace("%APPDATA%",
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        StringComparison.OrdinalIgnoreCase);
                }

                return expanded;
            }
            catch
            {
                return path;
            }
        }

        public static bool ValidatePath(string path)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var testFile = Path.Combine(directory, "test.tmp");
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
