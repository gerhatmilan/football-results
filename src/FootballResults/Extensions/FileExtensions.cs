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
    }
}
