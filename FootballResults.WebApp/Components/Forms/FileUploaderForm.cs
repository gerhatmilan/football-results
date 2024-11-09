using FootballResults.WebApp.Services.Files;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Forms
{
    public class FileUploaderForm : FormBase, IAsyncDisposable
    {
        
        protected override void ResetErrorMessages() { }

        protected FileUploadService FileUploadService { get; set; } = default!;

        protected string TemporaryFileName { get; set; } = Guid.NewGuid().ToString();

        protected void Initialize(string uploadDirectory, int maxAllowedBytes, string[] allowedFiles)
        {
            FileUploadService = new FileUploadService(uploadDirectory: uploadDirectory,
                maxAllowedBytes: maxAllowedBytes, allowedFiles: allowedFiles);
        }

        public async ValueTask DisposeAsync()
        {
            await FileUploadService.DeletePreviousUploadsByNameAsync(TemporaryFileName);
        }
    }
}
