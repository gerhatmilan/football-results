using FootballResults.Models.General;
using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services.Files;

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
        protected IConfiguration Configuration { get; set; } = default!;

        protected SettingsModel SettingsModel { get; set; } = new SettingsModel();

        protected string? ImageErrorMessage { get; set; }

        protected ModifyUserResult Result { get; set; }

        protected override void ResetErrorMessages()
        {
            ImageErrorMessage = null;
            Result = ModifyUserResult.None;
            FileUploadService = new FileUploadService(uploadDirectory: Configuration.GetValue<String>("Directories:ProfilePictures")!,
                               maxAllowedBytes: 1000000, allowedFiles: new string[] { "image/png", "image/jpeg" });
        }

        protected override void OnParametersSet()
        {
            SettingsModel.Username = User?.Username;
            SettingsModel.ProfilePicturePath = User?.ProfilePicturePath;
        }

        protected override void OnInitialized()
        {
            base.Initialize(uploadDirectory: Configuration.GetValue<String>("Directories:ProfilePictures")!,
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
                    NavigationManager.Refresh(forceReload: true);
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

                var (success, retVal) = await FileUploadService.UploadFileAsync(file: file, newFileName: TemporaryFileName);

                if (success)
                {
                    SettingsModel.ProfilePicturePath = retVal;
                }
                else
                {
                    ImageErrorMessage = retVal;
                }

                StateHasChanged();
            }
        }
    }
}
