using FootballResults.DataAccess;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.Models.Api.FootballApi.Responses;
using FootballResults.Models.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FootballResults.Models.Updaters
{
    [Updater]
    [SupportedModes(UpdaterMode.SpecificTeam, UpdaterMode.BasedOnLastUpdate)]
    public class SquadUpdater : Updater<SquadsResponse, SquadsResponseItem>
    {
        protected override UpdaterSpecificSettings UpdaterSpecificSettingsForTeam => _apiConfig.DataFetch.SquadForTeam;

        public SquadUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<TopScorerUpdater> logger)
            : base(serviceScopeFactory, logger) { }

        protected override async Task UpdateBasedOnLastUpdateAsync(TimeSpan maximumElapsedTimeSinceLastUpdate)
        {
            _logger.LogInformation($"{GetType().Name} starting...");

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                foreach (var team in _dbContext.Teams.OrderBy(t => t.ID))
                {
                    if (team.SquadLastUpdate == null || team.SquadLastUpdate < DateTime.UtcNow - maximumElapsedTimeSinceLastUpdate)
                    {
                        await UpdateForTeamAsync(team.ID);

                        if (!UpdaterSpecificSettingsForTeam.LoadDataFromBackup)
                        {
                            await DelayApiCallAsync();
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Squad data for team {team.Name} is up-to-date.");
                    }
                }
            }

            _logger.LogInformation($"{GetType().Name} has finished. Press any key to continue...");
        }

        protected override void ProcessData(IEnumerable<SquadsResponseItem> responseItems)
        {
            var existingPlayers = _dbContext.Players
                .Include(p => p.Team)
                .ToList();

            foreach (var responseItem in responseItems)
            {
                if (responseItem.Team.ID == null)
                {
                    _logger.LogWarning("Invalid team for squad, skipping...");
                    continue;
                }

                DataAccess.Entities.Football.Team existingTeam = _dbContext.Teams
                    .FirstOrDefault(team => team.ID == responseItem.Team.ID);

                if (existingTeam == null)
                {
                    _logger.LogWarning("Team does not exist in the database for this squad, skipping...");
                    continue;
                }

                foreach (Player mappedPlayer in responseItem.Players.Select(MapPlayer))
                {
                    if (mappedPlayer == null)
                    {
                        _logger.LogWarning($"Invalid player for team {responseItem.Team.Name}, skipping...");
                        continue;
                    }

                    mappedPlayer.TeamID = existingTeam.ID;
                    mappedPlayer.Team = existingTeam;

                    var existingRecord = existingPlayers.FirstOrDefault(existingRecord => existingRecord.PlayerID == mappedPlayer.PlayerID
                        && existingRecord.TeamID == mappedPlayer.TeamID);

                    if (existingRecord == null)
                    {
                        existingPlayers.Add(mappedPlayer);
                        Add(mappedPlayer);
                    }
                    else if (!existingRecord.Equals(mappedPlayer))
                        Update(existingRecord, mappedPlayer);
                }

                UpdateSquadLastUpdateFieldForTeam(existingTeam);
            }
        }

        protected void Add(Player player)
        {
            _logger.LogDebug($"Adding new player for team {player.Team.Name}: {player.Name}");
            _dbContext.Players.Add(player);
        }

        protected void Update(Player existingRecord, Player responseRecord)
        {
            _logger.LogDebug($"Updating player {existingRecord.Name} for team {existingRecord.Team.Name}:"
                + (existingRecord.ID != responseRecord.ID ? $"\n\tID: {existingRecord.ID} -> {responseRecord.ID}" : "")
                + (existingRecord.TeamID != responseRecord.TeamID ? $"\n\tTeamID: {existingRecord.TeamID} -> {responseRecord.TeamID}" : "")
                + (existingRecord.Name != responseRecord.Name ? $"\n\tName: {existingRecord.Name} -> {responseRecord.Name}" : "")
                + (existingRecord.Age != responseRecord.Age ? $"\n\tAge: {existingRecord.Age} -> {responseRecord.Age}" : "")
                + (existingRecord.Number != responseRecord.Number ? $"\n\tNumber: {existingRecord.Number} -> {responseRecord.Number}" : "")
                + (existingRecord.Position != responseRecord.Position ? $"\n\tPosition: {existingRecord.Position} -> {responseRecord.Position}" : "")
                + (existingRecord.PhotoLink != responseRecord.PhotoLink ? $"\n\tPhotoLink: {existingRecord.PhotoLink} -> {responseRecord.PhotoLink}" : ""));

            existingRecord.CopyFrom(responseRecord);
        }

        public static bool ValidatePlayer(SquadPlayer responseItem)
        {
            return responseItem.ID.HasValue
                && !string.IsNullOrEmpty(responseItem.Name);
        }

        public static Player MapPlayer(SquadPlayer responseItem)
        {
            if (ValidatePlayer(responseItem))
            {
                return new Player
                {
                    PlayerID = responseItem.ID!.Value,
                    Name = responseItem.Name,
                    Age = responseItem.Age,
                    Number = responseItem.Number,
                    Position = responseItem.Position,
                    PhotoLink = responseItem.Photo,
                };
            }
            else
            {
                return null;
            }
        }

        private void UpdateSquadLastUpdateFieldForTeam(DataAccess.Entities.Football.Team team)
        {
            team.SquadLastUpdate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        }
    }
}