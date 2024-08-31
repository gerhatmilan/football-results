using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballResults.DataAccess.Entities.Predictions
{
    public class Prediction : Entity
    {
        /// <summary>
        /// ID of the participation the prediction is made in
        /// </summary>
        public int ParticipationID { get; set; }

        /// <summary>
        /// ID of the match the prediction is made for
        /// </summary>
        public int MatchID { get; set; }

        /// <summary>
        /// Guess of the home team goals
        /// </summary>
        public int HomeTeamGoals { get; set; }

        /// <summary>
        /// Guess of the away team goals
        /// </summary>
        public int AwayTeamGoals { get; set; }

        /// <summary>
        /// Whether the points for this prediction have been given to the user
        /// </summary>
        public bool PointsGiven { get; set; }
        
        /// <summary>
        /// Date and time of the prediction
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Participation the prediction is made in
        /// </summary>
        public Participation Participation { get; set; }

        /// <summary>
        /// Prediction game the prediction is made in
        /// </summary>
        [NotMapped]
        public PredictionGame PredictionGame => Participation?.PredictionGame;

        /// <summary>
        /// User who made the prediction
        /// </summary>
        [NotMapped]
        public User User => Participation?.User;

        /// <summary>
        /// Match the prediction is made for
        /// </summary>
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
            if (PredictionGame != null)
            {
                if (IsExactScorelineReward())
                    return PredictionGame.ExactScorelineReward;
                else if (IsOutcomeReward())
                    return PredictionGame.OutcomeReward;
                else if (IsGoalCountReward())
                    return PredictionGame.GoalCountReward;
                else if (IsGoalDifferenceReward())
                    return PredictionGame.GoalDifferenceReward;
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
