namespace FootballResults.DataAccess.Entities
{
    public class EndpointConfig : EntityWithID
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string BackupPath { get; set; }
        public bool LoadDataFromBackup { get; set; }
    }
}
