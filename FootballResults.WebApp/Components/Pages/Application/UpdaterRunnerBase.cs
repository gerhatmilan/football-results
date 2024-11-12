using FootballResults.DataAccess.Entities;
using FootballResults.Models.Application;
using FootballResults.Models.Updaters;
using FootballResults.Models.Updaters.Services;
using FootballResults.Models.ViewModels.Application;
using FootballResults.WebApp.Components.Forms;
using FootballResults.WebApp.Services.Application;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.Application
{
    public class UpdaterRunnerBase : FormBase
    {
        [Inject]
        protected IUpdaterRunnerService UpdaterRunnerService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IApplicationService ApplicationService { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "updater")]
        protected string? Updater { get; set; }

        [SupplyParameterFromQuery(Name = "mode")]
        protected string? Mode { get; set; }

        protected UpdaterRunnerFormModel UpdaterRunnerFormModel { get; set; } = new UpdaterRunnerFormModel();
        protected ApplicationSettingsFormModel ApplicationSettingsFormModel { get; set; } = new ApplicationSettingsFormModel();

        protected IEnumerable<EndpointConfig> EndpointConfigs { get; set; } = default!;

        protected override void ResetErrorMessages()
        {
            UpdaterRunnerFormModel.ResetMessages();
            ApplicationSettingsFormModel.ResetMessages();
        }

        protected override void OnInitialized()
        {
            if (Updater == null && Mode == null)
            {
                return;
            }
            else if ((Updater != null && Mode == null) || (Updater == null && Mode != null))
            {
                NavigationManager.NavigateTo("/updater-runner");
                return;
            }

            Type? updaterType = IUpdaterRunnerService.AvailableUpdaters.FirstOrDefault(i => i.Name.ToLower().Equals(Updater!.ToLower()));
                
            if (updaterType == null)
            {
                NavigationManager.NavigateTo("/updater-runner");
                return;
            }


            if (!IUpdater.GetSupportedModesForType(updaterType).Any(i => i.ToString().ToLower().Equals(Mode!.ToLower())))
            {
                NavigationManager.NavigateTo("/updater-runner");
                return;
            }

            UpdaterRunnerService.SetUpdater(updaterType!.Name);
            UpdaterRunnerService.SelectedMode = IUpdater.GetSupportedModesForType(updaterType).First(i => i.ToString().ToLower().Equals(Mode));
            UpdaterRunnerFormModel.ModeSelected = true;
        }

        protected override async Task OnInitializedAsync()
        {
            EndpointConfigs = await ApplicationService.GetEndpointConfigsAsync();
        }

        protected void OnOptionSelected(string updaterName, UpdaterMode mode)
        {
            NavigationManager.NavigateTo($"updater-runner?updater={updaterName.ToLower()}&mode={mode.ToString().ToLower()}", forceLoad: true);
        }

        protected async Task SubmitAsync()
        {
            ResetErrorMessages();
            DisableForm();

            try
            {
                if (UpdaterRunnerFormModel.ModeParameterInteger != null)
                {
                    await UpdaterRunnerService.RunUpdaterAsync(UpdaterRunnerFormModel.ModeParameterInteger);
                }
                else if (UpdaterRunnerService.SelectedMode == UpdaterMode.SpecificDate || UpdaterRunnerService.SelectedMode == UpdaterMode.SpecificDateActiveLeagues)
                {
                    await UpdaterRunnerService.RunUpdaterAsync(UpdaterRunnerFormModel.ModeParameterDateTime);
                }
                else if (UpdaterRunnerService.SelectedMode == UpdaterMode.BasedOnLastUpdate)
                {
                    await UpdaterRunnerService.RunUpdaterAsync(UpdaterRunnerFormModel.ModeParameterTimeSpan);
                }
                else
                {
                    await UpdaterRunnerService.RunUpdaterAsync();
                }

                UpdaterRunnerFormModel.Success = true;
            }
            catch (Exception ex)
            {
                UpdaterRunnerFormModel.Error = true;
                UpdaterRunnerFormModel.ErrorMessage = "Updater failed: " + ex.Message;
            }

            await EnableForm();
        }

        protected async Task SaveAsync()
        {
            ResetErrorMessages();
            DisableForm();

            try
            {
                await ApplicationService.SaveConfig(new Config() { EndpointConfigs = EndpointConfigs });
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
