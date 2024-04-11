
using FootballResults.Models.Football;
using FootballResults.Models.Users;

namespace FootballResults.Models.Predictions
{
    public class Prediction
    {
        public int UserID { get; set; }
        public int GameID { get; set; }
        public int MatchID { get; set; }
        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }
        public bool PointsGiven { get; set; }
        public DateTime? PredictionDate { get; set; }
        public User User { get; set; }
        public PredictionGame Game { get; set; }
        public Match Match { get; set; }

        public bool IsExactScorelineReward()
        {
            if (Match == null || Match.HomeTeamGoals == null || Match.AwayTeamGoals == null)
                return false;

            return HomeTeamGoals == Match.HomeTeamGoals && AwayTeamGoals == Match.AwayTeamGoals;
        }

        public bool IsOutcomeReward()
        {
            if (Match == null || Match.HomeTeamGoals == null || Match.AwayTeamGoals == null)
                return false;

            return (HomeTeamGoals > AwayTeamGoals && Match.HomeTeamGoals > Match.AwayTeamGoals)
                || (HomeTeamGoals < AwayTeamGoals && Match.HomeTeamGoals < Match.AwayTeamGoals)
                || (HomeTeamGoals == AwayTeamGoals && Match.HomeTeamGoals == Match.AwayTeamGoals);
        }

        public bool IsGoalCountReward()
        {
            if (Match == null || Match.HomeTeamGoals == null || Match.AwayTeamGoals == null)
                return false;

            return (HomeTeamGoals == Match.HomeTeamGoals) || (AwayTeamGoals == Match.AwayTeamGoals);
        }

        public bool IsGoalDifferenceReward()
        {
            if (Match == null || Match.HomeTeamGoals == null || Match.AwayTeamGoals == null)
                return false;

            return Math.Abs(HomeTeamGoals - AwayTeamGoals) == Math.Abs(Match.HomeTeamGoals.Value - Match.AwayTeamGoals.Value);
        }

        public int CalculatePoints()
        {
            if (Game != null)
            {
                if (IsExactScorelineReward())
                    return Game.ExactScorelineReward;
                else if (IsOutcomeReward())
                    return Game.OutcomeReward;
                else if (IsGoalCountReward())
                    return Game.GoalCountReward;
                else if (IsGoalDifferenceReward())
                    return Game.GoalDifferenceReward;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
