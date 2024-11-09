namespace FootballResults.Models.Files
{
    public static class FileManager
    {
        public static async Task DeleteFileAsync(string filePath)
        {
            await Task.Run(() => File.Delete(filePath));
        }

        public static async Task MoveFileAsync(string oldPath, string newPath)
        {
            await Task.Run(() => File.Move(oldPath, newPath));
        }

        public static async Task DeleteFilesWithNameAsync(string path, string fileName)
        {
            var files = Directory.GetFiles(path, $"{fileName}.*");
            foreach (var file in files)
                await DeleteFileAsync(file);
        }

        public static async Task DownloadFileAsync(string url, string savePath)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));

                    using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
            }
        }
    }
}
