using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.Models.Predictions;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FootballResults.Models.General;
using FootballResults.WebApp.Services.Files;

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
        protected IConfiguration Configuration { get; set; } = default!;

        [Parameter]
        public User User { get; set; } = default!;

        protected CreateGameModel CreateGameModel { get; set; } = new CreateGameModel();

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
            base.Initialize(uploadDirectory: Configuration.GetValue<String>("Directories:PredictionGamePictures")!,
                maxAllowedBytes: 1000000, allowedFiles: new string[] { "image/png", "image/jpeg" });

            try
            {
                CreateGameModel.PicturePath = Configuration.GetValue<String>("Images:PredictionGameDefault")!;
                var leagues = await LeagueService.GetLeaguesAsync();

                CreateGameModel.IncludedLeagues = new List<Pair<League, bool>>();
                foreach (League league in leagues.OrderByDescending(l => l.Type).ThenBy(l => l.Name))
                {
                    CreateGameModel.IncludedLeagues.Add(new Pair<League, bool>(league, false));
                }
            }
            catch (HttpRequestException)
            {
                NavigationManager.NavigateTo("/Error", true);
            }
        }


        protected bool ValidateIncludedLeagues()
        {
            // at least one league needs to be selected
            if (CreateGameModel.IncludedLeagues.Count(pair => pair.Second == true) == 0)
            {
                IncludedLeaguesErrorMessage = "At least one league needs to be selected";
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

            PredictionGame? createdGame = await PredictionGameService.CreatePredictionGameAsync(User, CreateGameModel);
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
                FileUploadService uploadService = new FileUploadService(uploadDirectory: Configuration.GetValue<String>("Directories:PredictionGamePictures")!,
                    maxAllowedBytes: 1000000, allowedFiles: new string[] { "image/png", "image/jpeg" });

                var (success, retVal) = await uploadService.UploadFileAsync(file: file, newFileName: TemporaryFileName);

                if (success)
                {
                    CreateGameModel.PicturePath = retVal;
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
