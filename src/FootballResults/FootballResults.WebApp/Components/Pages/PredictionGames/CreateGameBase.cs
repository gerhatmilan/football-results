using FootballResults.Models.Football;
using FootballResults.Models.Users;
using FootballResults.Models.Predictions;
using FootballResults.WebApp.Services.Football;
using FootballResults.WebApp.Services.Predictions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FootballResults.WebApp.Components.Pages.PredictionGames
{
    public partial class CreateGameBase : ComponentBase
    {
        private const string DEFAULT_IMAGE = "prediction-games/backgrounds/default.jpg";

        [Inject]
        protected IPredictionGameService PredictionGameService { get; set; } = default!;

        [Inject]
        protected ILeagueService LeagueService { get; set; } = default!;

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public User User { get; set; } = default!;

        protected CreateGameModel CreateGameModel { get; set; } = new CreateGameModel();

        protected string? NotCreatedErrorMessage { get; set; }
        protected string? IncludedLeaguesErrorMessage { get; set; }
        protected string? ImageErrorMessage { get; set; }
        protected string SelectedImage { get; set; } = DEFAULT_IMAGE;

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

        protected async void Submit()
        {
            if (!ValidateIncludedLeagues())
                return;

            var success = await PredictionGameService.CreatePredictionGameAsync(User, CreateGameModel);
            if (success)
            {
                NavigationManager.NavigateTo("/prediction-games", true);
            }
            else
            {
                NotCreatedErrorMessage = "The game could not be created. Please try again later.";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
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
                    SelectedImage = DEFAULT_IMAGE;
                }
            }
        }
    }
}
