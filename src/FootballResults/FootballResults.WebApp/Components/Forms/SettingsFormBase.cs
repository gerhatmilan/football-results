using FootballResults.Models.General;
using FootballResults.Models.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services.Files;

namespace FootballResults.WebApp.Components.Forms
{
    public class SettingsFormBase : FormBase
    {
        [CascadingParameter(Name = "User")]
        public User? User { get; set; }

        [Inject]
        protected IUserService UserService { get; set; } = default!;

        [Inject]
        protected IFileUploadService FileUploadService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        protected SettingsModel SettingsModel { get; set; } = new SettingsModel();

        protected string? SelectedImage { get; set; }

        protected string? ImageErrorMessage { get; set; }

        protected string? ErrorMessage { get; set; }

        protected override void ResetErrorMessages()
        {
            ImageErrorMessage = null;
            ErrorMessage = null;
        }

        protected override async Task OnParametersSetAsync()
        {
            SettingsModel.Username = User?.Username;

            if (User!.ProfilePicturePath != null)
            {
                var profilePicture = await ImageManager.LoadImageAsync(User.ProfilePicturePath);
                SelectedImage = $"data:image/png;base64,{Convert.ToBase64String(profilePicture)}";
            }
        }

        protected async Task SubmitAsync()
        {
            if (User == null)
            {
                ErrorMessage = "An error has occurred. Please try again.";
                return;
            }
            else if (SettingsModel.Username == User.Username && SettingsModel.ProfilePicture == null)
                return;
            else
            {
                ResetErrorMessages();
                DisableForm();

                if (await UserService.ModifyUserAsync(User, SettingsModel))
                {
                    NavigationManager.NavigateTo("/", true);
                }
                else
                {
                    ErrorMessage = "An error has occurred. Please try again.";
                }

                await EnableForm();
            }   
        }
        protected async Task OnImageSelectedAsync(InputFileChangeEventArgs e)
        {
            ResetErrorMessages();
            var file = e.File;

            if (file != null)
            {
                try
                {
                    var buffer = new byte[file.Size];
                    await file.OpenReadStream().ReadAsync(buffer);
                    var imageBase64 = Convert.ToBase64String(buffer);
                    SelectedImage = $"data:{file.ContentType};base64,{imageBase64}";
                    SettingsModel.ProfilePicture = buffer;

                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    ImageErrorMessage = ex.Message;
                }
            }
        }
    }
}
