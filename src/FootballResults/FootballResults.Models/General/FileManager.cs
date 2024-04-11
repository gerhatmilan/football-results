using FootballResults.Models.Users;namespace FootballResults.Models.General
{
    public class FileManager
    {
        private const string ROOT_DIRECTORY = "wwwroot";

        private static async Task DeleteFileAsync(string filePath)
        {
            await Task.Run(() => File.Delete(filePath));
        }

        public static async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (var sourceStream = new FileStream(Path.Combine(ROOT_DIRECTORY, sourcePath), FileMode.Open, FileAccess.Read))
            using (var destinationStream = new FileStream(Path.Combine(ROOT_DIRECTORY, destinationPath), FileMode.CreateNew, FileAccess.Write))
                await sourceStream.CopyToAsync(destinationStream);
        }

        public static async Task MoveFileAsync(string oldPath, string newPath)
        {
            await Task.Run(() => File.Move(Path.Combine(ROOT_DIRECTORY, oldPath), Path.Combine(ROOT_DIRECTORY, newPath)));
        }

        public static async Task DeleteFilesWithNameAsync(string path, string fileName)
        {
            var files = Directory.GetFiles(Path.Combine(ROOT_DIRECTORY, path), $"{fileName}.*");
            foreach (var file in files)
                await DeleteFileAsync(file);
        }
    }
}
