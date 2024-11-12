using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Users;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using FootballResults.WebApp.Services.Files;
using FootballResults.Models.ViewModels.Users;
using FootballResults.DataAccess.Entities;
using FootballResults.WebApp.Services.Application;
using FootballResults.Models.Application;
using FootballResults.WebApp.Services.Helpers;
using FootballResults.DataAccess.Models;
using FootballResults.Models.ViewModels.Application;

namespace FootballResults.WebApp.Components.Forms
{
    public class ApplicationSettingsFormBase : FormBase
    {
        [CascadingParameter(Name = "User")]
        public User? User { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IApplicationService ApplicationService { get; set; } = default!;

        protected Config Config { get; set; } = default!;

        protected ApplicationSettingsFormModel ApplicationSettingsFormModel { get; set; } = new ApplicationSettingsFormModel();

        protected override void ResetErrorMessages()
        {
            ApplicationSettingsFormModel.ResetMessages();
        }

        protected override async Task OnInitializedAsync()
        {
            Config = await ApplicationService.GetConfigAsync();
            await ApplicationService.InitializeApplicationSettingsFormModelAsync(Config, ApplicationSettingsFormModel);
        }

        protected async Task SubmitAsync()
        {
            ResetErrorMessages();
            DisableForm();

            try
            {
                await ApplicationService.CopyApplicationSettingsFromModelAsync(Config, ApplicationSettingsFormModel);
                await ApplicationService.SaveConfig(Config);
                ApplicationSettingsFormModel.Success = true;
            }
            catch (Exception)
            {
                ApplicationSettingsFormModel.Error = true;
            }

            await EnableForm();
        }
    }
}
