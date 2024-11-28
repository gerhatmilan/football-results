namespace Extensions
{
    public class FileExtensions
    {
        public static void WriteAllText(string path, string? contents, bool createDirectory)
        {
            if (createDirectory)
            {
                string? directory = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            File.WriteAllText(path, contents);
        }

        public static char? GetPathSeparator(string path)
        {
            if (path.Contains('\\'))
            {
                return '\\';
            }
            else if (path.Contains('/'))
            {
                return '/';
            }
            else
            {
                return null;
            }
        }

        public static string GetNormalizedPath(string path)
        {
            char? separator = GetPathSeparator(path);

            if (separator == null || separator == Path.DirectorySeparatorChar)
            {
                return path;
            }

            return path.Replace(separator.Value, Path.DirectorySeparatorChar);
        }
    }
}
