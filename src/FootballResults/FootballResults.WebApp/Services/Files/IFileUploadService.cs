using Microsoft.AspNetCore.Components.Forms;

namespace FootballResults.WebApp.Services.Files
{
    public interface IFileUploadService
    {
        Task<(bool, string)> UploadFileAsync(IBrowserFile file, string newFileName);
        Task DeletePreviousUploadsByNameAsync(string fileName);
    }
}
