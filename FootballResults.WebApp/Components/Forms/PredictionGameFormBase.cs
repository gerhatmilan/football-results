using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FootballResults.WebApp.Services.Files;
using Microsoft.Extensions.Options;
using FootballResults.Models.Config;
using FootballResults.Models.ViewModels.PredictionGames;

namespace FootballResults.WebApp.Components.Forms
{
    public partial class PredictionGameFormBase : FileUploaderForm
    {
        [Inject]
        protected IPredictionGameService PredictionGameService { get; set; } = default!;

        [Inject]
        protected ILeagueService LeagueService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected IOptions<ApplicationConfig> ApplicationSettings { get; set; } = default!;

        [Parameter]
        public User User { get; set; } = default!;

        protected CreatePredictionGameModel CreateGameModel { get; set; } = new CreatePredictionGameModel();

        protected string? NotCreatedErrorMessage { get; set; }
        protected string? IncludedLeaguesErrorMessage { get; set; }
        protected string? ImageErrorMessage { get; set; }

        protected override void ResetErrorMessages()
        {
            NotCreatedErrorMessage = null;
            IncludedLeaguesErrorMessage = null;
            ImageErrorMessage = null;
        }

        protected override async Task OnInitializedAsync()
        {
            base.Initialize(uploadDirectory: ApplicationSettings.Value.PredictionGamePicturesDirectory,
                maxAllowedBytes: 1000000, allowedFiles: new string[] { "image/png", "image/jpeg" });

            try
            {
                var leagues = await LeagueService.GetLeaguesAsync();

                CreateGameModel.IncludedLeagues = new List<IncludedLeague>();
                foreach (League league in leagues.OrderByDescending(l => l.Type).ThenBy(l => l.Name))
                {
                    CreateGameModel.IncludedLeagues.Add(new IncludedLeague { League = league, Included = false });
                }
            }
            catch (HttpRequestException)
            {
                NavigationManager.NavigateTo("/error", true);
            }
        }


        protected bool ValidateIncludedLeagues()
        {
            // at least one league needs to be selected
            if (!CreateGameModel.IncludedLeagues.Any(includedLeague => includedLeague.Included))
            {
                IncludedLeaguesErrorMessage = "At least one league needs to be selected";
                return false;
            }
            else if (CreateGameModel.IncludedLeagues.Any(i => i.Included && i.League.CurrentSeason == null))
            {
                IncludedLeaguesErrorMessage = "One of the selected leagues does not have a season in progress";
                return false;
            }
            else
            {
                IncludedLeaguesErrorMessage = null;
                return true;
            }
        }

        protected async Task SubmitAsync()
        {
            DisableForm();

            if (!ValidateIncludedLeagues())
            {
                await EnableForm();
                return;
            }

            PredictionGame? createdGame = await PredictionGameService.CreatePredictionGameAsync(User.ID, CreateGameModel);
            if (createdGame != null)
            {
                NavigationManager.NavigateTo("/prediction-games", true);
            }
            else
            {
                NotCreatedErrorMessage = "The game could not be created. Please try again later.";
            }

            await EnableForm();
        }

        protected async Task OnImageSelectedAsync(InputFileChangeEventArgs e)
        {
            ImageErrorMessage = null;
            var file = e.File;

            if (User != null && file != null)
            {
                FileUploadService uploadService = new FileUploadService(uploadDirectory: ApplicationSettings.Value.PredictionGamePicturesDirectory,
                    maxAllowedBytes: 1000000, allowedFiles: new string[] { "image/png", "image/jpeg" });

                FileUploadResult result = await uploadService.UploadFileAsync(file: file, newFileName: TemporaryFileName);
                if (result.Success)
                {
                    CreateGameModel.PicturePath = result.Path;
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
