using FootballResults.Models.Files;
using Microsoft.AspNetCore.Components.Forms;

namespace FootballResults.WebApp.Services.Files
{
    public class FileUploadService : IFileUploadService
    {
        private readonly string _uploadDirectory;
        private int _maxAllowedBytes;
        private string[] _allowedFiles;
        private const string ROOT_DIRECTORY = "wwwroot";

        public FileUploadService(string uploadDirectory, int maxAllowedBytes, string[] allowedFiles)
        {
            _uploadDirectory = uploadDirectory;
            _maxAllowedBytes = maxAllowedBytes;
            _allowedFiles = allowedFiles;
        }

        public async Task<(bool, string)> UploadFileAsync(IBrowserFile file, string newFileName)
        {
            var combinedPath = Path.Combine(ROOT_DIRECTORY, _uploadDirectory);
            if (!Directory.Exists(combinedPath))
            {
                Directory.CreateDirectory(combinedPath);
            }

            if (file.Size > _maxAllowedBytes)
            {
                return (false, $"The file size exceeds the maximum allowed size of {_maxAllowedBytes} bytes");
            }

            if (!_allowedFiles.Contains(file.ContentType))
            {
                return (false, "File type not allowed");
            }

            var fileExtension = Path.GetExtension(file.Name);
            var fileName = $"{newFileName}{fileExtension}";
            await using var fs = new FileStream(Path.Combine(combinedPath, fileName), FileMode.Create);
            await file.OpenReadStream(_maxAllowedBytes).CopyToAsync(fs);

            return (true, _uploadDirectory + "/" + fileName);
        }

        public async Task DeletePreviousUploadsByNameAsync(string fileName)
        {
            await FileManager.DeleteFilesWithNameAsync(_uploadDirectory, fileName);
        }
    }
}
