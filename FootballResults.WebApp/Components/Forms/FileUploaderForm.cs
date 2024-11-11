using FootballResults.WebApp.Services.Files;

namespace FootballResults.WebApp.Components.Forms
{
    public abstract class FileUploaderForm : FormBase, IAsyncDisposable
    {
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
