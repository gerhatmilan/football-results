namespace FootballResults.Models.General
{
    public class ImageManager
    {
        private const string ROOT_DIRECTORY = "wwwroot";
        public static async Task SaveImageAsync(byte[] image, string path, string fileName)
        {
            string fullPath = Path.Combine(ROOT_DIRECTORY, path);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            using (var stream = new FileStream(Path.Combine(fullPath, fileName), FileMode.Create))
            {
                await stream.WriteAsync(image, 0, image.Length);
            }
        }

        public static async Task<byte[]> LoadImageAsync(string path)
        {
            using (var stream = new FileStream(Path.Combine(ROOT_DIRECTORY, path), FileMode.Open))
            {
                var buffer = new byte[stream.Length];
                await stream.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
