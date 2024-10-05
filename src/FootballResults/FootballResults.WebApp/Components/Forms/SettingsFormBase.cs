using FootballResults.DataAccess.Entities.Users;
using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services.Files;
using Microsoft.Extensions.Options;
using FootballResults.Models.Config;

namespace FootballResults.WebApp.Components.Forms
{
    public class SettingsFormBase : FileUploaderForm
    {
        [CascadingParameter(Name = "User")]
        public User? User { get; set; }

        [Inject]
        protected IUserService UserService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IOptions<ApplicationConfig> ApplicationSettings { get; set; } = default!;

        protected SettingsModel SettingsModel { get; set; } = new SettingsModel();

        protected string? ImageErrorMessage { get; set; }

        protected ModifyUserResult Result { get; set; }

        protected override void ResetErrorMessages()
        {
            ImageErrorMessage = null;
            Result = ModifyUserResult.None;
        }

        protected override void OnParametersSet()
        {
            SettingsModel.Username = User?.Username;
            SettingsModel.ProfilePicturePath = User?.ProfilePicturePath;
        }

        protected override void OnInitialized()
        {
            base.Initialize(uploadDirectory: ApplicationSettings.Value.ProfilePicturesDirectory,
                maxAllowedBytes: 1000000, allowedFiles: new string[] { "image/png", "image/jpeg" });
        }

        protected async Task SubmitAsync()
        {
            if (User == null)
            {
                Result = ModifyUserResult.Error;
                return;
            }
            else if (SettingsModel.Username == User.Username && SettingsModel.ProfilePicturePath == User.ProfilePicturePath)
                return;
            else
            {
                ResetErrorMessages();
                DisableForm();

                if ((Result = await UserService.ModifyUserAsync(User, SettingsModel)) == ModifyUserResult.Success)
                {
                    NavigationManager.Refresh();
                }

                await EnableForm();
            }   
        }
        protected async Task OnImageSelectedAsync(InputFileChangeEventArgs e)
        {
            ResetErrorMessages();

            var file = e.File;

            if (User != null && file != null)
            {

                FileUploadResult result = await FileUploadService.UploadFileAsync(file: file, newFileName: TemporaryFileName);

                if (result.Success)
                {
                    SettingsModel.ProfilePicturePath = result.Path;
                }
                else
                {
                    ImageErrorMessage = result.Message;
                }

                StateHasChanged();
            }
        }
    }
}
