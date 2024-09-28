namespace FootballResults.Models.Predictions
{
    public class PredictionModel
    {
        public int? HomeTeamGoals { get; set; }

        public int? AwayTeamGoals { get; set; }

        public bool Valid => HomeTeamGoals.HasValue && AwayTeamGoals.HasValue;
    }
}
