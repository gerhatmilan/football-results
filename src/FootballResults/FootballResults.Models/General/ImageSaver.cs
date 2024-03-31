namespace FootballResults.Models.General
{
    public class ImageSaver
    {
        public static async Task SaveImageAsync(byte[] image, string path)
        {
            // save the picture to the file system, only the path will be saved in the database
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await stream.WriteAsync(image, 0, image.Length);
            }
        }
    }
}
