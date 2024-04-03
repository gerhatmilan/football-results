using FootballResults.Models.Users;
using Microsoft.AspNetCore.Components.Forms;

namespace FootballResults.WebApp.Services.Files
{
    public class FileUploadService : IFileUploadService
    {
        public Task<bool> UploadFileAsync(IBrowserFile file, int maxAllowedBytes, string[] allowedFormats)
        {
            throw new NotImplementedException();
            /*
            var uploadDirectory = Path.Combine(_environment.WebRootPath, "Uploads");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            if (file.Size > maxFileSize)
            {
                return (0, $"File: {file.Name} exceeds the maximum allowed file size.");
            }

            var fileExtension = Path.GetExtension(file.Name);
            if (!allowedExtensions.Contains(fileExtension))
            {
                return (0, $"File: {file.Name}, File type not allowed");
            }

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var path = Path.Combine(uploadDirectory, fileName);
            await using var fs = new FileStream(path, FileMode.Create);
            await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
            return (1, fileName);
            */
        }
    }
}
