using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FootballResults.WebApp.Services.Files;
using FootballResults.Models.ViewModels.PredictionGames;
using FootballResults.DataAccess.Entities;
using FootballResults.WebApp.Services.Application;

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
        protected IApplicationService ApplicationService { get; set; } = default!;

        [Parameter]
        public User User { get; set; } = default!;

        protected CreatePredictionGameModel CreateGameModel { get; set; } = new CreatePredictionGameModel();

        protected ApplicationConfig ApplicationConfig { get; set; } = default!;

        protected override void ResetErrorMessages()
        {
            CreateGameModel.ResetMessages();
        }

        protected override async Task OnInitializedAsync()
        {
            ApplicationConfig = await ApplicationService.GetApplicationConfigAsync();

            base.Initialize(uploadDirectory: ApplicationConfig.PredictionGamePicturesDirectory,
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
            catch (Exception)
            {
                NavigationManager.NavigateTo("/error", true);
            }
        }

        protected async Task SubmitAsync()
        {
            if (!CreateGameModel.ValidateIncludedLeagues() || CreateGameModel.ImageError)
            {
                return;
            }

            ResetErrorMessages();
            DisableForm();

            PredictionGame? createdGame = await PredictionGameService.CreatePredictionGameAsync(User.ID, CreateGameModel);
            if (createdGame != null)
            {
                NavigationManager.NavigateTo($"/prediction-games/{createdGame.ID}", true);
            }
            else
            {
                CreateGameModel.Error = true;
                await EnableForm();
            }
        }

        protected async Task OnImageSelectedAsync(InputFileChangeEventArgs e)
        {
            ResetErrorMessages();
            var file = e.File;

            if (User != null && file != null)
            {
                FileUploadService uploadService = new FileUploadService(uploadDirectory: ApplicationConfig.PredictionGamePicturesDirectory,
                    maxAllowedBytes: 1000000, allowedFiles: new string[] { "image/png", "image/jpeg" });

                FileUploadResult result = await uploadService.UploadFileAsync(file: file, newFileName: TemporaryFileName);
                if (result.Success)
                {
                    CreateGameModel.PicturePath = result.Path;
                }
                else
                {
                    CreateGameModel.ImageErrorMessage = result.Message;
                }

                StateHasChanged();
            }
        }
    }
}
