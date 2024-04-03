using Microsoft.AspNetCore.Components.Forms;

namespace FootballResults.WebApp.Services.Files
{
    public interface IFileUploadService
    {
        Task<bool> UploadFileAsync(IBrowserFile file, int maxAllowedBytes, string[] allowedFormats);
    }
}
