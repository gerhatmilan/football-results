namespace FootballResults.Models.ViewModels.PredictionGames
{
    public class PredictionModel
    {
        public int? HomeTeamGoals { get; set; }

        public int? AwayTeamGoals { get; set; }

        public string HomeGoalsState { get; set; } = "default";

        public string AwayGoalsState { get; set; } = "default";

        public bool MatchStartedError { get; set; } = false;

        public bool BothFilled => HomeTeamGoals.HasValue && AwayTeamGoals.HasValue;

        public bool Valid => BothFilled && HomeTeamGoals >= 0 && AwayTeamGoals >= 0 && !MatchStartedError;

        public void EnableSuccessIndicator()
        {
            HomeGoalsState = "success";
            AwayGoalsState = "success";
        }

        public void ResetIndicators()
        {
            HomeGoalsState = "default";
            AwayGoalsState = "default";
        }
    }
}
