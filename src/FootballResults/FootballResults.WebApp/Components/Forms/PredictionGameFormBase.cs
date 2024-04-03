using FootballResults.Models.Football;
using FootballResults.Models.Users;
using FootballResults.Models.Predictions;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FootballResults.Models.General;

namespace FootballResults.WebApp.Components.Forms
{
    public partial class PredictionGameFormBase : FormBase
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
        protected string? SelectedImage { get; set; } 

        protected override void ResetErrorMessages()
        {
            NotCreatedErrorMessage = null;
            IncludedLeaguesErrorMessage = null;
            ImageErrorMessage = null;
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

        protected override async Task OnInitializedAsync()
        {
            try
            {
                SelectedImage = Configuration.GetValue<string>("Images:PredictionGameDefault");
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

        protected async Task OnImageSelectedAsync(InputFileChangeEventArgs e)
        {
            ImageErrorMessage = null;
            var file = e.File;

            if (file != null)
            {
                try
                {
                    var buffer = new byte[file.Size];
                    await file.OpenReadStream().ReadAsync(buffer);
                    var imageBase64 = Convert.ToBase64String(buffer);
                    SelectedImage = $"data:{file.ContentType};base64,{imageBase64}";
                    CreateGameModel.Picture = buffer;
                }
                catch (IOException ex)
                {
                    ImageErrorMessage = ex.Message + " Using default image.";
                    SelectedImage = Configuration.GetValue<string>("Images:PredictionGameDefault");
                }
            }
        }
    }
}
