using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services.Files;
using FootballResults.Models.ViewModels.Users;
using FootballResults.WebApp.Services.Predictions;
using FootballResults.DataAccess.Entities;
using FootballResults.WebApp.Services.Application;

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

        [Inject] IApplicationService ApplicationService { get; set; } = default!;

        protected ApplicationConfig ApplicationConfig { get; set; } = default!;

        protected SettingsModel SettingsModel { get; set; } = new SettingsModel();

        protected override void ResetErrorMessages()
        {
            SettingsModel.ResetMessages();
        }

        protected override void OnParametersSet()
        {
            SettingsModel.Username = User?.Username;
            SettingsModel.ImagePath = User?.ProfilePicturePath;
        }

        protected override async Task OnInitializedAsync()
        {
            ApplicationConfig = await ApplicationService.GetApplicationConfigAsync();

            base.Initialize(uploadDirectory: ApplicationConfig.ProfilePicturesDirectory,
                maxAllowedBytes: 1000000, allowedFiles: new string[] { "image/png", "image/jpeg" });
        }

        protected async Task SubmitAsync()
        {
            if (SettingsModel.Username == User!.Username && SettingsModel.ImagePath == User.ProfilePicturePath)
                return;
            else if (!SettingsModel.ImageError)
            {
                ResetErrorMessages();
                DisableForm();

                if (await UserService.ModifyUserAsync(User, SettingsModel))
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
                try
                {
                    FileUploadResult result = await FileUploadService.UploadFileAsync(file: file, newFileName: TemporaryFileName);

                    if (result.Success)
                    {
                        SettingsModel.ImagePath = result.Path;
                    }
                    else
                    {
                        SettingsModel.ImageError = true;
                        SettingsModel.ImageErrorMessage = result.Message;
                    }
                }
                catch (Exception)
                {
                    SettingsModel.Error = true;
                }

                StateHasChanged();
            }
        }
    }
}
