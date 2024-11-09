using Microsoft.AspNetCore.Components.Forms;

namespace FootballResults.WebApp.Services.Files
{
    public interface IFileUploadService
    {
        Task<FileUploadResult> UploadFileAsync(IBrowserFile file, string newFileName);
        Task DeletePreviousUploadsByNameAsync(string fileName);
    }
}
