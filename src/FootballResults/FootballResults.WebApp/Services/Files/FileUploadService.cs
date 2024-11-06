using FootballResults.Models.Files;
using Microsoft.AspNetCore.Components.Forms;

namespace FootballResults.WebApp.Services.Files
{
    public class FileUploadService : IFileUploadService
    {
        private readonly string _uploadDirectory;
        private int _maxAllowedBytes;
        private string[] _allowedFiles;
        private const string WWWROOT = "wwwroot";

        public int MaxAllowedBytes => _maxAllowedBytes;

        public FileUploadService(string uploadDirectory, int maxAllowedBytes, string[] allowedFiles)
        {
            _uploadDirectory = uploadDirectory;
            _maxAllowedBytes = maxAllowedBytes;
            _allowedFiles = allowedFiles;
        }

        public async Task<FileUploadResult> UploadFileAsync(IBrowserFile file, string newFileName)
        {
            var combinedPath = Path.Combine(WWWROOT, _uploadDirectory);
            Directory.CreateDirectory(combinedPath);

            if (file.Size > _maxAllowedBytes)
            {
                return new FileUploadResult()
                {
                    Success = false,
                    Message = $"The file size exceeds the maximum allowed size of {_maxAllowedBytes} bytes",
                    Path = null
                };
            }

            if (!_allowedFiles.Contains(file.ContentType))
            {
                return new FileUploadResult()
                {
                    Success = false,
                    Message = "File type not allowed",
                    Path = null
                };
            }

            var fileExtension = Path.GetExtension(file.Name);
            var fileName = $"{newFileName}{fileExtension}";

            await using (var fileStream = new FileStream(Path.Combine(combinedPath, fileName), FileMode.Create))
            {
                await using (var readStream = file.OpenReadStream(_maxAllowedBytes))
                {
                    await readStream.CopyToAsync(fileStream);
                }
            }

            return new FileUploadResult()
            {
                Success = true,
                Message = "File was successfully updated",
                Path = Path.Combine(_uploadDirectory, fileName)
            };
        }

        public async Task DeletePreviousUploadsByNameAsync(string fileName)
        {
            await FileManager.DeleteFilesWithNameAsync(Path.Combine(WWWROOT, _uploadDirectory), fileName);
        }
    }
}
