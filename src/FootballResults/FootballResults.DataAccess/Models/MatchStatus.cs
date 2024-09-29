namespace FootballResults.DataAccess.Models
{
    /// <summary>
    /// https://www.api-football.com/documentation-v3#tag/Fixtures/operation/get-fixtures-rounds
    /// </summary>
    public static class MatchStatus
    {
        public const string ToBeDecided = "TBD";
        public const string NotStarted = "NS";
        public const string FirstHalf = "1H";
        public const string HalfTime = "HT";
        public const string SecondHalf = "2H";
        public const string ExtraTime = "ET";
        public const string BreakTime = "BT";
        public const string PenaltiesInProgress = "P";
        public const string Suspended = "SUSP";
        public const string Interrupted = "INT";
        public const string FullTime = "FT";
        public const string AwardedExtraTime = "AET";
        public const string Penalties = "PEN";
        public const string Postponed = "PST";
        public const string Cancelled = "CANC";
        public const string Abandoned = "ABD";
        public const string TechnicalLoss = "AWD";
        public const string WalkOver = "WO";
        public const string Live = "LIVE";
    }
}
