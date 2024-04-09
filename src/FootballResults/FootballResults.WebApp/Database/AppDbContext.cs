using FootballResults.Models.Users;
using FootballResults.Models.Predictions;
using Microsoft.EntityFrameworkCore;
using FootballResults.Models.Football;

namespace FootballResults.WebApp.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        #region Football schema

        public DbSet<Country> Countries { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<AvailableSeason> AvailableSeasons { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<LeagueStanding> LeagueStandings { get; set; }
        public DbSet<TopScorer> TopScorers { get; set; }
        public DbSet<Player> Players { get; set; }

        #endregion

        #region Users schema

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FavoriteLeague> FavoriteLeagues { get; set; }
        public DbSet<FavoriteTeam> FavoriteTeams { get; set; }

        #endregion

        #region Predictions schema

        public DbSet<PredictionGame> PredictionGames { get; set; }
        public DbSet<GameLeague> IncludedLeagues { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<GameStanding> GameStandings { get; set; }
        public DbSet<Participation> Participations { get; set; }

        #endregion

        private void SetTableNames(ModelBuilder modelBuilder)
        {
            #region Football schema

            modelBuilder.Entity<Country>()
                .ToTable(name: "countries", schema: "football");

            modelBuilder.Entity<League>()
                .ToTable(name: "leagues", schema: "football");

            modelBuilder.Entity<Team>()
                .ToTable(name: "teams", schema: "football");

            modelBuilder.Entity<Venue>()
                .ToTable(name: "venues", schema: "football");

            modelBuilder.Entity<Match>()
                .ToTable(name: "matches", schema: "football");

            modelBuilder.Entity<AvailableSeason>()
                .ToTable(name: "available_seasons", schema: "football");

            modelBuilder.Entity<LeagueStanding>()
                .ToTable(name: "standings", schema: "football");

            modelBuilder.Entity<TopScorer>()
                .ToTable(name: "top_scorers", schema: "football");

            modelBuilder.Entity<Player>()
                .ToTable(name: "players", schema: "football");

            #endregion

            #region Users schema
            modelBuilder.Entity<User>()
                .ToTable(name: "users", schema: "users");

            modelBuilder.Entity<Message>()
                .ToTable(name: "messages", schema: "users");

            modelBuilder.Entity<FavoriteLeague>()
                .ToTable(name: "favorite_leagues", schema: "users");

            modelBuilder.Entity<FavoriteTeam>()
                .ToTable(name: "favorite_teams", schema: "users");
            #endregion

            #region Predictions schema
            modelBuilder.Entity<PredictionGame>()
                .ToTable(name: "prediction_games", schema: "predictions");
            modelBuilder.Entity<GameLeague>()
                .ToTable(name: "included_leagues", schema: "predictions");
            modelBuilder.Entity<Prediction>()
                .ToTable(name: "predictions", schema: "predictions");
            modelBuilder.Entity<GameStanding>()
                .ToTable(name: "standings", schema: "predictions");
            modelBuilder.Entity<Participation>()
                .ToTable(name: "participations", schema: "predictions");
            #endregion
        }

        private void SetColumns(ModelBuilder modelBuilder)
        {
            #region Football schema

            #region Countries
            modelBuilder.Entity<Country>()
                .Property(c => c.CountryID)
                .HasColumnName("country_id")
                .IsRequired(true);
            modelBuilder.Entity<Country>()
                .Property(c => c.FlagLink)
                .HasColumnName("flag_link")
                .IsRequired(false);
            #endregion

            #region Leagues
            modelBuilder.Entity<League>()
                .Property(l => l.LeagueID)
                .HasColumnName("league_id")
                .IsRequired(true);
            modelBuilder.Entity<League>()
                .Property(l => l.CountryID)
                .HasColumnName("country_id")
                .IsRequired(false);
            modelBuilder.Entity<League>()
                .Property(l => l.Name)
                .HasColumnName("name")
                .IsRequired(true);
            modelBuilder.Entity<League>()
                .Property(l => l.Type)
                .HasColumnName("type")
                .IsRequired(true);
            modelBuilder.Entity<League>()
                .Property(l => l.CurrentSeason)
                .HasColumnName("current_season")
                .IsRequired(true);
            modelBuilder.Entity<League>()
                .Property(l => l.LogoLink)
                .HasColumnName("logo_link")
                .IsRequired(false);
            #endregion

            #region Teams
            modelBuilder.Entity<Team>()
                .Property(t => t.TeamID)
                .HasColumnName("team_id")
                .IsRequired(true);
            modelBuilder.Entity<Team>()
                .Property(t => t.CountryID)
                .HasColumnName("country_id")
                .IsRequired(true);
            modelBuilder.Entity<Team>()
                .Property(t => t.VenueID)
                .HasColumnName("venue_id")
                .IsRequired(false);
            modelBuilder.Entity<Team>()
                .Property(t => t.Name)
                .HasColumnName("name")
                .IsRequired(true);
            modelBuilder.Entity<Team>()
                .Property(t => t.ShortName)
                .HasColumnName("short_name")
                .IsRequired(false);
            modelBuilder.Entity<Team>()
                .Property(t => t.LogoLink)
                .HasColumnName("logo_link")
                .IsRequired(false);
            modelBuilder.Entity<Team>()
                .Property(t => t.National)
                .HasColumnName("national")
                .IsRequired(true);
            #endregion

            #region Venues
            modelBuilder.Entity<Venue>()
                .Property(v => v.VenueID)
                .HasColumnName("venue_id")
                .IsRequired(true);
            modelBuilder.Entity<Venue>()
                .Property(v => v.CountryID)
                .HasColumnName("country_id")
                .IsRequired(true);
            modelBuilder.Entity<Venue>()
                .Property(v => v.City)
                .HasColumnName("city")
                .IsRequired(false);
            modelBuilder.Entity<Venue>()
                .Property(v => v.Name)
                .HasColumnName("name")
                .IsRequired(false);
            #endregion

            #region Available seasons
            modelBuilder.Entity<AvailableSeason>()
                .Property(s => s.LeagueID)
                .HasColumnName("league_id")
                .IsRequired(true);
            modelBuilder.Entity<AvailableSeason>()
                .Property(s => s.Season)
                .HasColumnName("season")
                .IsRequired(true);
            #endregion

            #region Matches
            modelBuilder.Entity<Match>()
                .Property(m => m.MatchID)
                .HasColumnName("match_id")
                .IsRequired(true);
            modelBuilder.Entity<Match>()
                .Property(m => m.Date)
                .HasColumnName("date")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(m => m.VenueID)
                .HasColumnName("venue_id")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(m => m.LeagueID)
                .HasColumnName("league_id")
                .IsRequired(true);
            modelBuilder.Entity<Match>()
                .Property(m => m.Season)
                .HasColumnName("season")
                .IsRequired(true);
            modelBuilder.Entity<Match>()
                .Property(m => m.Round)
                .HasColumnName("round")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(m => m.HomeTeamID)
                .HasColumnName("home_team_id")
                .IsRequired(true);
            modelBuilder.Entity<Match>()
                .Property(m => m.AwayTeamID)
                .HasColumnName("away_team_id")
                .IsRequired(true);
            modelBuilder.Entity<Match>()
                .Property(m => m.Status)
                .HasColumnName("status")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(m => m.Minute)
                .HasColumnName("minute")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(m => m.HomeTeamGoals)
                .HasColumnName("home_team_goals")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(m => m.AwayTeamGoals)
                .HasColumnName("away_team_goals")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(m => m.LastUpdate)
                .HasColumnName("last_update")
                .IsRequired(false);
            #endregion

            #region Standings
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.LeagueID)
                .HasColumnName("league_id")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Season)
                .HasColumnName("season")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.TeamID)
                .HasColumnName("team_id")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Rank)
                .HasColumnName("rank")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Group)
                .HasColumnName("group")
                .IsRequired(false);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Points)
                .HasColumnName("points")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Played)
                .HasColumnName("played")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Wins)
                .HasColumnName("wins")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Draws)
                .HasColumnName("draws")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Losses)
                .HasColumnName("losses")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Scored)
                .HasColumnName("scored")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.Conceded)
                .HasColumnName("conceded")
                .IsRequired(true);
            modelBuilder.Entity<LeagueStanding>()
                .Property(s => s.LastUpdate)
                .HasColumnName("last_update")
                .IsRequired(false);
            #endregion

            #region Top scorers
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.LeagueID)
                .HasColumnName("league_id")
                .IsRequired(true);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.Season)
                .HasColumnName("season")
                .IsRequired(true);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.Rank)
                .HasColumnName("rank")
                .IsRequired(true);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.PlayerName)
                .HasColumnName("player_name")
                .IsRequired(true);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.PhotoLink)
                .HasColumnName("photo_link")
                .IsRequired(false);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.TeamID)
                .HasColumnName("team_id")
                .IsRequired(true);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.Played)
                .HasColumnName("played")
                .IsRequired(false);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.Goals)
                .HasColumnName("goals")
                .IsRequired(true);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.Assists)
                .HasColumnName("assists")
                .IsRequired(false);
            modelBuilder.Entity<TopScorer>()
                .Property(ts => ts.LastUpdate)
                .HasColumnName("last_update")
                .IsRequired(false);
            #endregion

            #region Players
            modelBuilder.Entity<Player>()
                .Property(p => p.PlayerID)
                .HasColumnName("player_id")
                .IsRequired(true);
            modelBuilder.Entity<Player>()
                .Property(p => p.TeamID)
                .HasColumnName("team_id")
                .IsRequired(true);
            modelBuilder.Entity<Player>()
                .Property(p => p.Name)
                .HasColumnName("name")
                .IsRequired(true);
            modelBuilder.Entity<Player>()
                .Property(p => p.Age)
                .HasColumnName("age")
                .IsRequired(false);
            modelBuilder.Entity<Player>()
                .Property(p => p.Number)
                .HasColumnName("number")
                .IsRequired(false);
            modelBuilder.Entity<Player>()
                .Property(p => p.Position)
                .HasColumnName("position")
                .IsRequired(false);
            modelBuilder.Entity<Player>()
                .Property(p => p.PhotoLink)
                .HasColumnName("photo_link")
                .IsRequired(false);

            #endregion

            #endregion

            #region Users schema

            #region Users
            modelBuilder.Entity<User>()
                .Property(u => u.UserID)
                .HasColumnName("user_id")
                .ValueGeneratedOnAdd()
                .IsRequired(true);
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasColumnName("email")
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasColumnName("username")
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasColumnName("password")
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .Property(u => u.ProfilePicturePath)
                .HasColumnName("profile_pic_path")
                .ValueGeneratedNever()
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .Property(u => u.RegistrataionDate)
                .HasColumnName("registration_date")
                .ValueGeneratedNever()
                .IsRequired(false);
            #endregion

            #region Favorite leagues

            modelBuilder.Entity<FavoriteLeague>()
                .Property(fl => fl.UserID)
                .HasColumnName("user_id")
                .IsRequired(true);
            modelBuilder.Entity<FavoriteLeague>()
                .Property(fl => fl.LeagueID)
                .HasColumnName("league_id")
                .IsRequired(true);

            #endregion

            #region Favorite teams

            modelBuilder.Entity<FavoriteTeam>()
                .Property(ft => ft.UserID)
                .HasColumnName("user_id")
                .IsRequired(true);
            modelBuilder.Entity<FavoriteTeam>()
                .Property(ft => ft.TeamID)
                .HasColumnName("team_id")
                .IsRequired(true);

            #endregion

            #region Messages

            modelBuilder.Entity<Message>()
                .Property(m => m.MessageID)
                .HasColumnName("message_id")
                .ValueGeneratedOnAdd()
                .IsRequired(true);
            modelBuilder.Entity<Message>()
                .Property(m => m.UserID)
                .HasColumnName("user_id")
                .IsRequired(true);
            modelBuilder.Entity<Message>()
                .Property(m => m.SentAt)
                .HasColumnName("sent_at")
                .IsRequired(true);
            modelBuilder.Entity<Message>()
                .Property(m => m.Text)
                .HasColumnName("message")
                .IsRequired(false);
            modelBuilder.Entity<Message>()
                .Property(m => m.MatchID)
                .HasColumnName("match_id")
                .IsRequired(false);
            modelBuilder.Entity<Message>()
                .Property(m => m.PredictionGameID)
                .HasColumnName("prediction_game_id")
                .IsRequired(false);

            #endregion

            #endregion

            #region Predictions schema

            #region Prediction games

            modelBuilder.Entity<PredictionGame>()
               .Property(g => g.GameID)
               .HasColumnName("prediction_game_id")
               .ValueGeneratedOnAdd()
               .IsRequired(true);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.Name)
                .HasColumnName("name")
                .IsRequired(true);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.OwnerID)
                .HasColumnName("owner_id")
                .IsRequired(true);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.JoinKey)
                .HasColumnName("join_key")
                .IsRequired(true);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.Description)
                .HasColumnName("description")
                .IsRequired(false);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.ImagePath)
                .HasColumnName("image_path")
                .IsRequired(false);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.ExactScorelineReward)
                .HasColumnName("exact_scoreline_reward")
                .IsRequired(true);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.OutcomeReward)
                .HasColumnName("outcome_reward")
                .IsRequired(true);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.GoalCountReward)
                .HasColumnName("goal_count_reward")
                .IsRequired(true);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.GoalDifferenceReward)
                .HasColumnName("goal_difference_reward")
                .IsRequired(true);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.CreatedAt)
                .HasColumnName("created_at")
                .ValueGeneratedNever()
                .IsRequired(false);
            modelBuilder.Entity<PredictionGame>()
                .Property(g => g.IsFinished)
                .HasColumnName("finished")
                .IsRequired(true);

            #endregion

            #region Included leagues
            modelBuilder.Entity<GameLeague>()
                .Property(il => il.GameID)
                .HasColumnName("prediction_game_id")
                .IsRequired(true);
            modelBuilder.Entity<GameLeague>()
                .Property(il => il.LeagueID)
                .HasColumnName("league_id")
                .IsRequired(true);
            modelBuilder.Entity<GameLeague>()
                .Property(il => il.Season)
                .HasColumnName("season")
                .IsRequired(true);
            #endregion

            #region Predictions
            modelBuilder.Entity<Prediction>()
                .Property(p => p.UserID)
                .HasColumnName("user_id")
                .IsRequired(true);
            modelBuilder.Entity<Prediction>()
                .Property(p => p.GameID)
                .HasColumnName("prediction_game_id")
                .IsRequired(true);
            modelBuilder.Entity<Prediction>()
                .Property(p => p.MatchID)
                .HasColumnName("match_id")
                .IsRequired(true);
            modelBuilder.Entity<Prediction>()
                .Property(p => p.HomeTeamGoals)
                .HasColumnName("home_team_goals")
                .IsRequired(true);
            modelBuilder.Entity<Prediction>()
                .Property(p => p.AwayTeamGoals)
                .HasColumnName("away_team_goals")
                .IsRequired(true);
            modelBuilder.Entity<Prediction>()
                .Property(p => p.PredictionDate)
                .HasColumnName("prediction_date")
                .IsRequired(false);
            #endregion

            #region Standings
            modelBuilder.Entity<GameStanding>()
                .Property(s => s.UserID)
                .HasColumnName("user_id")
                .IsRequired(true);
            modelBuilder.Entity<GameStanding>()
                .Property(s => s.GameID)
                .HasColumnName("prediction_game_id")
                .IsRequired(true);
            modelBuilder.Entity<GameStanding>()
                .Property(s => s.Points)
                .HasColumnName("points")
                .IsRequired(true);
            modelBuilder.Entity<GameStanding>()
                .Property(s => s.LastUpdate)
                .HasColumnName("last_update")
                .IsRequired(false);
            #endregion

            #region Participations
            modelBuilder.Entity<Participation>()
                .Property(p => p.GameID)
                .HasColumnName("prediction_game_id")
                .IsRequired(true);
            modelBuilder.Entity<Participation>()
                .Property(p => p.UserID)
                .HasColumnName("user_id")
                .IsRequired(true);
            modelBuilder.Entity<Participation>()
                .Property(p => p.JoinDate)
                .HasColumnName("join_date")
                .IsRequired(false);
            #endregion

            #endregion
        }

        private void SetPrimaryKeys(ModelBuilder modelBuilder)
        {
            #region Football schema

            modelBuilder.Entity<Country>()
               .HasKey(c => c.CountryID);

            modelBuilder.Entity<League>()
                .HasKey(l => l.LeagueID);

            modelBuilder.Entity<Team>()
                .HasKey(t => t.TeamID);

            modelBuilder.Entity<Venue>()
                .HasKey(v => v.VenueID);

            modelBuilder.Entity<Match>()
                .HasKey(m => m.MatchID);

            modelBuilder.Entity<AvailableSeason>()
                .HasKey(s => new { s.LeagueID, s.Season });

            modelBuilder.Entity<LeagueStanding>()
                .HasKey(s => new { s.LeagueID, s.Season, s.TeamID });

            modelBuilder.Entity<TopScorer>()
                .HasKey(ts => new { ts.LeagueID, ts.Season, ts.Rank });

            modelBuilder.Entity<Player>()
                .HasKey(p => new { p.PlayerID, p.TeamID });

            #endregion

            #region Users schema

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            modelBuilder.Entity<Message>()
                .HasKey(m => m.MessageID);

            modelBuilder.Entity<FavoriteLeague>()
                .HasKey(fl => new { fl.UserID, fl.LeagueID });

            modelBuilder.Entity<FavoriteTeam>()
                .HasKey(ft => new { ft.UserID, ft.TeamID });

            #endregion

            #region Predictions schema

            modelBuilder.Entity<PredictionGame>()
                .HasKey(g => g.GameID);

            modelBuilder.Entity<GameLeague>()
                .HasKey(il => new { il.GameID, il.LeagueID });

            modelBuilder.Entity<Prediction>()
                .HasKey(p => new { p.UserID, p.GameID, p.MatchID });

            modelBuilder.Entity<GameStanding>()
                .HasKey(s => new { s.UserID, s.GameID });

            modelBuilder.Entity<Participation>()
                .HasKey(p => new { p.GameID, p.UserID });

            #endregion
        }

        private void SetRelationShips(ModelBuilder modelBuilder)
        {
            #region Football schema

            #region Countries
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Leagues)
                .WithOne(l => l.Country)
                .HasForeignKey(l => l.CountryID);

            modelBuilder.Entity<Country>()
                .HasMany(c => c.Venues)
                .WithOne(v => v.Country)
                .HasForeignKey(v => v.CountryID);

            modelBuilder.Entity<Country>()
                .HasMany(c => c.Teams)
                .WithOne(t => t.Country)
                .HasForeignKey(t => t.CountryID);
            #endregion

            #region Leagues

            modelBuilder.Entity<League>()
                .HasMany(l => l.AvailableSeasons)
                .WithOne(s => s.League)
                .HasForeignKey(s => s.LeagueID);

            modelBuilder.Entity<League>()
                .HasMany(l => l.Matches)
                .WithOne(m => m.League)
                .HasForeignKey(m => m.LeagueID);

            modelBuilder.Entity<League>()
                .HasMany(l => l.Standings)
                .WithOne(s => s.League)
                .HasForeignKey(s => s.LeagueID);

            modelBuilder.Entity<League>()
                .HasMany(l => l.TopScorers)
                .WithOne(ts => ts.League)
                .HasForeignKey(ts => ts.LeagueID);

            #endregion

            #region Matches

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Venue)
                .WithMany()
                .HasForeignKey(m => m.VenueID)
                .IsRequired(false);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamID);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamID);

            #endregion

            #region Teams

            modelBuilder.Entity<Team>()
                .HasOne(t => t.Venue)
                .WithMany()
                .HasForeignKey(t => t.VenueID)
                .IsRequired(false);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.HomeMatches)
                .WithOne(m => m.HomeTeam)
                .HasForeignKey(m => m.HomeTeamID);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.AwayMatches)
                .WithOne(m => m.AwayTeam)
                .HasForeignKey(m => m.AwayTeamID);

            #endregion

            #endregion


            // User N : N League (favorite leagues)
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteLeagues)
                .WithMany(l => l.UsersWhoBookmarked)
                .UsingEntity<FavoriteLeague>(
                    l => l.HasOne(l => l.League)
                        .WithMany(l => l.UserLeagues)
                        .HasForeignKey(l => l.LeagueID),
                    r => r.HasOne(r => r.User)
                        .WithMany(u => u.UserLeagues)
                        .HasForeignKey(r => r.UserID));

            // User N : N Team (favorite teams)
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteTeams)
                .WithMany(t => t.UsersWhoBookmarked)
                .UsingEntity<FavoriteTeam>(
                    l => l.HasOne(l => l.Team)
                        .WithMany(l => l.UserTeams)
                        .HasForeignKey(l => l.TeamID),
                    r => r.HasOne(r => r.User)
                        .WithMany(u => u.UserTeams)
                        .HasForeignKey(r => r.UserID));

            // User 1 : N Message
            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserID);

            // User N : N PredictionGame (participants in a prediction game)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Games)
                .WithMany(g => g.Players)
                .UsingEntity<Participation>(
                    l => l.HasOne(l => l.Game)
                        .WithMany(g => g.Participations)
                        .HasForeignKey(l => l.GameID),
                    r => r.HasOne(r => r.User)
                        .WithMany(u => u.Participations)
                        .HasForeignKey(r => r.UserID));

            // User 1 : N PredictionGame (created games by user)
            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedGames)
                .WithOne(g => g.Owner)
                .HasForeignKey(g => g.OwnerID);

            // PredictionGame N : N League (included leagues in competition)
            modelBuilder.Entity<PredictionGame>()
                .HasMany(g => g.Leagues)
                .WithMany(l => l.GamesWhereIncluded)
                .UsingEntity<GameLeague>(
                    l => l.HasOne(l => l.League)
                        .WithMany(g => g.GameLeagues)
                        .HasForeignKey(l => l.LeagueID),
                    r => r.HasOne(r => r.Game)
                        .WithMany(l => l.GameLeagues)
                        .HasForeignKey(r => r.GameID));

            // User 1 : N Prediction
            modelBuilder.Entity<User>()
                .HasMany(u => u.Predictions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserID);

            // User 1 : N GameStanding
            modelBuilder.Entity<User>()
                .HasMany(u => u.Standings)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserID);

            // PredictionGame 1 : N Prediction
            modelBuilder.Entity<PredictionGame>()
                .HasMany(g => g.Predictions)
                .WithOne(p => p.Game)
                .HasForeignKey(p => p.GameID);

            // PredictionGame 1 : N GameStanding
            modelBuilder.Entity<PredictionGame>()
                .HasMany(g => g.Standings)
                .WithOne(s => s.Game)
                .HasForeignKey(s => s.GameID);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SetTableNames(modelBuilder);
            SetColumns(modelBuilder);
            SetPrimaryKeys(modelBuilder);
            SetRelationShips(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is User && 
                    e.State == EntityState.Added);

            var messageEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Message && 
                    e.State == EntityState.Added);

            var predictionGameEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is PredictionGame && 
                    e.State == EntityState.Added);

            var predictionEntries = ChangeTracker
               .Entries()
               .Where(e => e.Entity is Prediction && (
                   e.State == EntityState.Added
                   || e.State == EntityState.Modified));

            var participationEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Participation &&
                    e.State == EntityState.Added);

            var standingsEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is GameStanding &&
                    (e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var user in userEntries)
            {
                ((User)user.Entity).RegistrataionDate = DateTime.UtcNow;
            }

            foreach (var message in messageEntries)
            {
                ((Message)message.Entity).SentAt = DateTime.UtcNow;
            }

            foreach (var predictionGame in predictionGameEntries)
            {
                ((PredictionGame)predictionGame.Entity).CreatedAt = DateTime.UtcNow;
            }

            foreach (var prediction in predictionEntries)
            {
                ((Prediction)prediction.Entity).PredictionDate = DateTime.UtcNow;
            }

            foreach (var participation in participationEntries)
            {
                ((Participation)participation.Entity).JoinDate = DateTime.UtcNow;
            }

            foreach (var standing in standingsEntries)
            {
                ((GameStanding)standing.Entity).LastUpdate = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync();
        }
    }
}