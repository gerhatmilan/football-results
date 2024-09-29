namespace FootballResults.DataAccess.Entities
{
    public class SystemInformation : EntityWithID
    {
        public DateTime? MatchesLastUpdateForCurrentDay { get; set; }
    }
}
