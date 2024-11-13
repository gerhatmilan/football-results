using Extensions;

namespace FootballResults.DataAccess.Entities
{
    public class EndpointConfig : EntityWithID
    {
        private string _backupPath;

        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string BackupPath
        {
            get => _backupPath;
            set => _backupPath = FileExtensions.GetNormalizedPath(value);
        }
        public bool LoadDataFromBackup { get; set; }
    }
}
