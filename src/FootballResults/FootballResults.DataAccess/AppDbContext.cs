using FootballResults.DataAccess.Entities;
using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Predictions;
using FootballResults.DataAccess.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FootballResults.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        #region Public schema

        public DbSet<SystemInformation> SystemInformation { get; set; }

        #endregion
        
        #region Football schema

        public DbSet<Country> Countries { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<LeagueSeason> LeagueSeasons { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<LeagueStanding> LeagueStandings { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TopScorer> TopScorers { get; set; }
        public DbSet<Venue> Venues { get; set; }

        #endregion

        #region Predictions schema

        public DbSet<Participation> Participations { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<PredictionGame> PredictionGames { get; set; }
        public DbSet<PredictionGameSeason> PredictionGameSeasons { get; set; }
        public DbSet<PredictionGameStanding> PredictionGameStandings { get; set; }

        #endregion

        #region Users schema

        public DbSet<FavoriteLeague> FavoriteLeagues { get; set; }
        public DbSet<FavoriteTeam> FavoriteTeams { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        #endregion

        private void SetTableNames(ModelBuilder modelBuilder)
        {
            #region Public schema

            modelBuilder.Entity<SystemInformation>()
                .ToTable(name: "system_information", schema: "public");

            #endregion

            #region Football schema

            modelBuilder.Entity<Country>()
                .ToTable(name: "country", schema: "football");

            modelBuilder.Entity<League>()
                .ToTable(name: "league", schema: "football");

            modelBuilder.Entity<LeagueSeason>()
                .ToTable(name: "league_season", schema: "football");

            modelBuilder.Entity<LeagueStanding>()
                .ToTable(name: "standing", schema: "football");
            
            modelBuilder.Entity<Match>()
                .ToTable(name: "match", schema: "football");

            modelBuilder.Entity<Player>()
                .ToTable(name: "player", schema: "football");

            modelBuilder.Entity<Team>()
                .ToTable(name: "team", schema: "football");

            modelBuilder.Entity<TopScorer>()
                .ToTable(name: "topscorer", schema: "football");

            modelBuilder.Entity<Venue>()
                .ToTable(name: "venue", schema: "football");

            #endregion

            #region Predictions schema

            modelBuilder.Entity<Participation>()
                .ToTable(name: "participation", schema: "predictions");

            modelBuilder.Entity<Prediction>()
                .ToTable(name: "prediction", schema: "predictions");

            modelBuilder.Entity<PredictionGame>()
                .ToTable(name: "prediction_game", schema: "predictions");

            modelBuilder.Entity<PredictionGameSeason>()
                .ToTable(name: "prediction_game_season", schema: "predictions");

            modelBuilder.Entity<PredictionGameStanding>()
                .ToTable(name: "standing", schema: "predictions");

            #endregion

            #region Users schema

            modelBuilder.Entity<FavoriteLeague>()
                .ToTable(name: "favorite_league", schema: "users");

            modelBuilder.Entity<FavoriteTeam>()
                .ToTable(name: "favorite_team", schema: "users");

            modelBuilder.Entity<Message>()
                .ToTable(name: "message", schema: "users");

            modelBuilder.Entity<User>()
                .ToTable(name: "user", schema: "users");

            #endregion
        }

        private void SetPrimaryKeys(ModelBuilder modelBuilder)
        {
            #region Public schema
            modelBuilder.Entity<SystemInformation>()
                .HasKey(e => e.ID);
            #endregion

            #region Football schema

            modelBuilder.Entity<Country>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<League>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<LeagueSeason>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<LeagueStanding>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<Match>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<Player>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<Team>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<TopScorer>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<Venue>()
                .HasKey(e => e.ID);

            #endregion

            #region Predictions schema

            modelBuilder.Entity<Participation>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<Prediction>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<PredictionGame>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<PredictionGameSeason>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<PredictionGameStanding>()
                .HasKey(e => e.ID);

            #endregion

            #region Users schema

            modelBuilder.Entity<FavoriteLeague>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<FavoriteTeam>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<Message>()
                .HasKey(e => e.ID);
            modelBuilder.Entity<User>()
                .HasKey(e => e.ID);

            #endregion
        }

        private void SetColumns(ModelBuilder modelBuilder)
        {
            #region Public schema

            #region SystemInformation
            modelBuilder.Entity<SystemInformation>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<SystemInformation>()
                .Property(e => e.MatchesLastUpdateForCurrentDay)
                .HasColumnName("matches_last_update_for_current_day")
                .HasColumnType("timestamp")
                .IsRequired(false);
            #endregion

            #endregion

            #region Football schema

            #region Country
            modelBuilder.Entity<Country>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("country_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Country>()
                .Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<Country>()
                .Property(e => e.FlagLink)
                .HasColumnName("flag_link")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<Country>()
                .HasIndex(e => e.Name)
                .IsUnique();
            #endregion

            #region League
            modelBuilder.Entity<League>()
                .Property(e => e.ID)
                .HasColumnName("league_id")
                .HasColumnType("int")
                .ValueGeneratedNever()
                .IsRequired();
            modelBuilder.Entity<League>()
                .Property(e => e.CountryID)
                .HasColumnName("country_id")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<League>()
                .Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<League>()
                .Property(e => e.Type)
                .HasColumnName("type")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<League>()
                .Property(e => e.LogoLink)
                .HasColumnName("logo_link")
                .HasColumnType("varchar")
                .IsRequired(false);
            #endregion

            #region LeagueSeason
            modelBuilder.Entity<LeagueSeason>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("league_season_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueSeason>()
                .Property(e => e.LeagueID)
                .HasColumnName("league_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueSeason>()
                .Property(e => e.Year)
                .HasColumnName("year")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueSeason>()
                .Property(e => e.InProgress)
                .HasColumnName("in_progress")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();
            modelBuilder.Entity<LeagueSeason>()
                .Property(e => e.StandingsLastUpdate)
                .HasColumnName("standings_last_update")
                .HasColumnType("timestamp")
                .IsRequired(false);
            modelBuilder.Entity<LeagueSeason>()
                .Property(e => e.TopScorersLastUpdate)
                .HasColumnName("topscorers_last_update")
                .HasColumnType("timestamp")
                .IsRequired(false);
            modelBuilder.Entity<LeagueSeason>()
                .Property(e => e.MatchesLastUpdate)
                .HasColumnName("matches_last_update")
                .HasColumnType("timestamp")
                .IsRequired(false);
            #endregion

            #region LeagueStanding
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("standing_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.LeagueSeasonID)
                .HasColumnName("league_season_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.TeamID)
                .HasColumnName("team_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Rank)
                .HasColumnName("rank")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Group)
                .HasColumnName("group")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Points)
                .HasColumnName("points")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Played)
                .HasColumnName("played")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Wins)
                .HasColumnName("wins")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Draws)
                .HasColumnName("draws")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Losses)
                .HasColumnName("losses")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Scored)
                .HasColumnName("scored")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .Property(e => e.Conceded)
                .HasColumnName("conceded")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<LeagueStanding>()
                .HasIndex(e => new { e.LeagueSeasonID, e.Group, e.TeamID })
                .IsUnique();
            #endregion

            #region Match

            modelBuilder.Entity<Match>()
                .Property(e => e.ID)
                .HasColumnName("match_id")
                .HasColumnType("int")
                .ValueGeneratedNever()
                .IsRequired();
            modelBuilder.Entity<Match>()
                .Property(e => e.LeagueSeasonID)
                .HasColumnName("league_season_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Match>()
                .Property(e => e.VenueID)
                .HasColumnName("venue_id")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(e => e.HomeTeamID)
                .HasColumnName("home_team_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Match>()
                .Property(e => e.AwayTeamID)
                .HasColumnName("away_team_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Match>()
                .Property(e => e.Round)
                .HasColumnName("round")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(e => e.Date)
                .HasColumnName("date")
                .HasColumnType("timestamp")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(e => e.Status)
                .HasColumnName("status")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(e => e.Minute)
                .HasColumnName("minute")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(e => e.HomeTeamGoals)
                .HasColumnName("home_team_goals")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(e => e.AwayTeamGoals)
                .HasColumnName("away_team_goals")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Match>()
                .Property(e => e.LastUpdate)
                .HasColumnName("last_update")
                .HasColumnType("timestamp")
                .IsRequired(false);
            #endregion

            #region Player
            modelBuilder.Entity<Player>()
                .Property(e => e.ID)
                .HasColumnName("id")
                .HasColumnType("int")
                .UseIdentityByDefaultColumn()
                .IsRequired();
            modelBuilder.Entity<Player>()
                .Property(e => e.PlayerID)
                .HasColumnName("player_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Player>()
                .Property(e => e.TeamID)
                .HasColumnName("team_id")
                .HasColumnType("int")
                .IsRequired(false);          
            modelBuilder.Entity<Player>()
                .Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<Player>()
                .Property(e => e.Age)
                .HasColumnName("age")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Player>()
                .Property(e => e.Number)
                .HasColumnName("number")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Player>()
                .Property(e => e.Position)
                .HasColumnName("position")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<Player>()
                .Property(e => e.PhotoLink)
                .HasColumnName("photo_link")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<Player>()
                .HasIndex(e => new { e.PlayerID, e.TeamID })
                .IsUnique();
            #endregion

            #region Team
            modelBuilder.Entity<Team>()
                .Property(e => e.ID)
                .HasColumnName("team_id")
                .HasColumnType("int")
                .ValueGeneratedNever()
                .IsRequired();
            modelBuilder.Entity<Team>()
                .Property(e => e.CountryID)
                .HasColumnName("country_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Team>()
                .Property(e => e.VenueID)
                .HasColumnName("venue_id")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Team>()
                .Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<Team>()
                .Property(e => e.ShortName)
                .HasColumnName("short_name")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<Team>()
                .Property(e => e.LogoLink)
                .HasColumnName("logo_link")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<Team>()
                .Property(e => e.National)
                .HasColumnName("national")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();
            modelBuilder.Entity<Team>()
                .Property(e => e.SquadLastUpdate)
                .HasColumnName("squad_last_update")
                .HasColumnType("timestamp")
                .IsRequired(false);
            #endregion

            #region Topscorer
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("topscorer_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.LeagueSeasonID)
                .HasColumnName("league_season_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.TeamID)
                .HasColumnName("team_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.Rank)
                .HasColumnName("rank")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.PlayerName)
                .HasColumnName("player_name")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.PhotoLink)
                .HasColumnName("photo_link")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.Played)
                .HasColumnName("played")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.Goals)
                .HasColumnName("goals")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<TopScorer>()
                .Property(e => e.Assists)
                .HasColumnName("assists")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<TopScorer>()
                .HasIndex(e => new { e.LeagueSeasonID, e.Rank })
                .IsUnique();
            #endregion

            #region Venue
            modelBuilder.Entity<Venue>()
                .Property(e => e.ID)
                .HasColumnName("venue_id")
                .HasColumnType("int")
                .ValueGeneratedNever()
                .IsRequired();
            modelBuilder.Entity<Venue>()
                .Property(e => e.CountryID)
                .HasColumnName("country_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Venue>()
                .Property(e => e.City)
                .HasColumnName("city")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<Venue>()
                .Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("varchar")
                .IsRequired(false);
            #endregion

            #endregion

            #region Predictions schema

            #region Participation
            modelBuilder.Entity<Participation>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("participation_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Participation>()
                .Property(e => e.PredictionGameID)
                .HasColumnName("prediction_game_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Participation>()
                .Property(e => e.UserID)
                .HasColumnName("user_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Participation>()
                .Property(p => p.StandingID)
                .HasColumnName("standing_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Participation>()
                .Property(e => e.JoinDate)
                .HasColumnName("join_date")
                .HasColumnType("timestamp")
                .IsRequired(false);
            modelBuilder.Entity<Participation>()
                .HasIndex(e => new { e.PredictionGameID, e.UserID })
                .IsUnique();
            #endregion

            #region Prediction
            modelBuilder.Entity<Prediction>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("prediction_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Prediction>()
                .Property(e => e.ParticipationID)
                .HasColumnName("participation_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Prediction>()
                .Property(e => e.MatchID)
                .HasColumnName("match_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Prediction>()
                .Property(e => e.HomeTeamGoals)
                .HasColumnName("home_team_goals")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Prediction>()
                .Property(e => e.AwayTeamGoals)
                .HasColumnName("away_team_goals")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Prediction>()
                .Property(e => e.PointsGiven)
                .HasColumnName("points_given")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();
            modelBuilder.Entity<Prediction>()
                .Property(e => e.Date)
                .HasColumnName("prediction_date")
                .HasColumnType("timestamp")
                .IsRequired(false);
            modelBuilder.Entity<Prediction>()
                .HasIndex(e => new { e.ParticipationID, e.MatchID })
                .IsUnique();
            #endregion
            
            #region PredictionGame

            modelBuilder.Entity<PredictionGame>()
               .Property(e => e.ID)
               .UseIdentityByDefaultColumn()
               .HasColumnName("prediction_game_id")
               .HasColumnType("int")
               .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.OwnerID)
                .HasColumnName("owner_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.JoinKey)
                .HasColumnName("join_key")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .IsRequired(false);
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.ImagePath)
                .HasColumnName("image_path")
                .HasColumnType("varchar")
                .IsRequired(false);
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.ExactScorelineReward)
                .HasColumnName("exact_scoreline_reward")
                .HasColumnType("int")
                .HasDefaultValue(10)
                .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.OutcomeReward)
                .HasColumnName("outcome_reward")
                .HasColumnType("int")
                .HasDefaultValue(8)
                .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.GoalCountReward)
                .HasColumnName("goal_count_reward")
                .HasDefaultValue(5)
                .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.GoalDifferenceReward)
                .HasColumnName("goal_difference_reward")
                .HasColumnType("int")
                .HasDefaultValue(3)
                .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .IsRequired(false);
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.StandingsLastUpdate)
                .HasColumnName("standings_last_update")
                .HasColumnType("timestamp")
                .IsRequired(false);
            modelBuilder.Entity<PredictionGame>()
                .Property(e => e.Finished)
                .HasColumnName("finished")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();
            modelBuilder.Entity<PredictionGame>()
                .HasIndex(e => e.JoinKey)
                .IsUnique();
            #endregion
            
            #region PredictionGameSeason
            modelBuilder.Entity<PredictionGameSeason>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("predicton_game_season_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<PredictionGameSeason>()
                .Property(e => e.PredictionGameID)
                .HasColumnName("prediction_game_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<PredictionGameSeason>()
                .Property(e => e.LeagueSeasonID)
                .HasColumnName("league_season_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<PredictionGameSeason>()
                .HasIndex(e => new { e.PredictionGameID, e.LeagueSeasonID })
                .IsUnique();
            #endregion
            
            #region Standing
            modelBuilder.Entity<PredictionGameStanding>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("standing_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<PredictionGameStanding>()
                .Property(e => e.ParticipationID)
                .HasColumnName("participation_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<PredictionGameStanding>()
                .Property(e => e.Points)
                .HasColumnName("points")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<PredictionGameStanding>()
                .HasIndex(e => e.ParticipationID)
                .IsUnique();
            #endregion

            #endregion

            #region Users schema

            #region FavoriteLeague
            modelBuilder.Entity<FavoriteLeague>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("favorite_league_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<FavoriteLeague>()
                .Property(e => e.UserID)
                .HasColumnName("user_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<FavoriteLeague>()
                .Property(e => e.LeagueID)
                .HasColumnName("league_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<FavoriteLeague>()
                .HasIndex(e => new { e.UserID, e.LeagueID })
                .IsUnique();
            #endregion

            #region FavoriteTeam
            modelBuilder.Entity<FavoriteTeam>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("favorite_team_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<FavoriteTeam>()
                .Property(e => e.UserID)
                .HasColumnName("user_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<FavoriteTeam>()
                .Property(e => e.TeamID)
                .HasColumnName("team_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<FavoriteTeam>()
                .HasIndex(e => new { e.UserID, e.TeamID })
                .IsUnique();
            #endregion

            #region User
            modelBuilder.Entity<User>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("user_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .HasColumnName("email")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(e => e.Username)
                .HasColumnName("username")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .HasColumnName("password")
                .HasColumnType("varchar")
                .IsRequired();
            modelBuilder.Entity<User>()
                .Property(e => e.ProfilePicturePath)
                .HasColumnName("profile_pic_path")
                .HasColumnType("varchar")
                .ValueGeneratedNever()
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .Property(e => e.RegistrataionDate)
                .HasColumnName("registration_date")
                .HasColumnType("timestamp")
                .ValueGeneratedNever()
                .IsRequired(false);
            modelBuilder.Entity<User>()
                .HasIndex(e => e.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(e => e.Username)
                .IsUnique();
            #endregion

            #region Message
            modelBuilder.Entity<Message>()
                .Property(e => e.ID)
                .UseIdentityByDefaultColumn()
                .HasColumnName("message_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Message>()
                .Property(e => e.UserID)
                .HasColumnName("user_id")
                .HasColumnType("int")
                .IsRequired();
            modelBuilder.Entity<Message>()
                .Property(e => e.MatchID)
                .HasColumnName("match_id")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Message>()
                .Property(e => e.PredictionGameID)
                .HasColumnName("prediction_game_id")
                .HasColumnType("int")
                .IsRequired(false);
            modelBuilder.Entity<Message>()
                .Property(e => e.SentAt)
                .HasColumnName("sent_at")
                .HasColumnType("timestamp")
                .IsRequired(false);
            modelBuilder.Entity<Message>()
                .Property(e => e.Text)
                .HasColumnName("text")
                .HasColumnType("text")
                .IsRequired();
            #endregion

            #endregion
        }

        private void SetRelationships(ModelBuilder modelBuilder)
        {
            // Country 1 : N League (leagues in a country)
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Leagues)
                .WithOne(l => l.Country)
                .HasForeignKey(l => l.CountryID)
                .IsRequired();

            // Country 1 : N Team (teams in a country)
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Teams)
                .WithOne(t => t.Country)
                .HasForeignKey(t => t.CountryID)
                .IsRequired();
            
            // Country 1 : N Venue (venues in a country)
            modelBuilder.Entity<Country>()
                .HasMany(c => c.Venues)
                .WithOne(v => v.Country)
                .HasForeignKey(v => v.CountryID)
                .IsRequired();

            // league seasons for a league
            modelBuilder.Entity<League>()
                .HasMany(l => l.LeagueSeasons)
                .WithOne(ls => ls.League)
                .HasForeignKey(ls => ls.LeagueID);

            // LeagueSeason 1 : N Match (matches in a leagueseason)
            modelBuilder.Entity<LeagueSeason>()
                .HasMany(ls => ls.Matches)
                .WithOne(m => m.LeagueSeason)
                .HasForeignKey(m => m.LeagueSeasonID)
                .IsRequired();

            // LeagueSeason 1 : N LeagueStanding (standings in a leagueseason)
            modelBuilder.Entity<LeagueSeason>()
                .HasMany(ls => ls.Standings)
                .WithOne(s => s.LeagueSeason)
                .HasForeignKey(s => s.LeagueSeasonID)
                .IsRequired();

            // LeagueSeason 1 : N TopScorer (topscorers in a leagueseason)
            modelBuilder.Entity<LeagueSeason>()
                .HasMany(ls => ls.TopScorers)
                .WithOne(ts => ts.LeagueSeason)
                .HasForeignKey(ts => ts.LeagueSeasonID)
                .IsRequired();

            // Match N : 1 Venue (matches at a venue)
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Venue)
                .WithMany(v => v.Matches)
                .HasForeignKey(m => m.VenueID)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Match N : 1 (Home)Team (matches of the home team)
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamID)
                .IsRequired();

            // Match N : 1 (Away)Team (matches of the away team)
            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamID)
                .IsRequired();

            // Player N : 1 Team (players in a team)
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Squad)
                .HasForeignKey(p => p.TeamID)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // LeagueStanding N : 1 Team (standings of a team)
            modelBuilder.Entity<LeagueStanding>()
                .HasOne(s => s.Team)
                .WithMany(t => t.Standings)
                .HasForeignKey(s => s.TeamID)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired();

            // Team N : 1 Venue (venue of a team)
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Venue)
                .WithMany(v => v.Teams)
                .HasForeignKey(t => t.VenueID)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // TopScorer N : 1 Team (topscorers of a team)
            modelBuilder.Entity<TopScorer>()
                .HasOne(ts => ts.Team)
                .WithMany(t => t.TopScorers)
                .HasForeignKey(ts => ts.TeamID)
                .IsRequired();



            // Participation 1 : N Prediction (predictions of a participation)
            modelBuilder.Entity<Participation>()
                .HasMany(pa => pa.Predictions)
                .WithOne(pr => pr.Participation)
                .HasForeignKey(pr => pr.ParticipationID)
                .IsRequired();

            // Participation 1 : 1 Standing (standing of a participation)
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Standing)
                .WithOne(s => s.Participation)
                .HasForeignKey<PredictionGameStanding>(s => s.ParticipationID)
                .IsRequired();

            // PredictionGame N : N User (Participation entity for connectiong prediction games and users)
            modelBuilder.Entity<PredictionGame>()
                .HasMany(g => g.Players)
                .WithMany(p => p.PredictionGames)
                .UsingEntity<Participation>
                (
                    par => par.HasOne(par => par.User)
                        .WithMany(u => u.Participations)
                        .HasForeignKey(par => par.UserID)
                        .IsRequired(),
                    par => par.HasOne(par => par.PredictionGame)
                        .WithMany(g => g.Participations)
                        .HasForeignKey(par => par.PredictionGameID)
                        .IsRequired()
                );

            // PredictionGame N : 1 User (prediction games created by an user)
            modelBuilder.Entity<PredictionGame>()
                .HasOne(g => g.Owner)
                .WithMany(u => u.CreatedPredictionGames)
                .HasForeignKey(g => g.OwnerID)
                .IsRequired();

            // PredictionGame N : N LeagueSeason (PredictionGameSeason entity for connecting prediction games with league seasons)
            modelBuilder.Entity<PredictionGame>()
                .HasMany(pg => pg.LeagueSeasons)
                .WithMany(ls => ls.PredictionGames)
                .UsingEntity<PredictionGameSeason>
                (
                    pgls => pgls.HasOne(pgls => pgls.LeagueSeason)
                        .WithMany(ls => ls.PredictionGameSeasons)
                        .HasForeignKey(pgls => pgls.LeagueSeasonID)
                        .IsRequired(),
                    pgls => pgls.HasOne(pgls => pgls.PredictionGame)
                        .WithMany(pg => pg.PredictionGameSeasons)
                        .HasForeignKey(pgls => pgls.PredictionGameID)
                        .IsRequired()
                );

            // Prediction N : 1 Match (predictions made for a match)
            modelBuilder.Entity<Prediction>()
                .HasOne(p => p.Match)
                .WithMany(m => m.Predictions)
                .HasForeignKey(p => p.MatchID)
                .IsRequired();



            // User N : N League (FavoriteLeague entity connecting users and leagues)
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteLeagues)
                .WithMany(l => l.UsersWhoBookmarked)
                .UsingEntity<FavoriteLeague>(
                    fl => fl.HasOne(fl => fl.League)
                        .WithMany(l => l.FavoriteLeagues)
                        .HasForeignKey(fl => fl.LeagueID)
                        .IsRequired(),
                    fl => fl.HasOne(fl => fl.User)
                        .WithMany(u => u.UserLeagues)
                        .HasForeignKey(fl => fl.UserID)
                        .IsRequired()
                 );

            // User N : N Team (FavoriteTeam entity connecting userse and teams)
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteTeams)
                .WithMany(t => t.UsersWhoBookmarked)
                .UsingEntity<FavoriteTeam>(
                    ft => ft.HasOne(ft => ft.Team)
                        .WithMany(t => t.FavoriteTeams)
                        .HasForeignKey(ft => ft.TeamID)
                        .IsRequired(),
                    ft => ft.HasOne(ft => ft.User)
                        .WithMany(u => u.UserTeams)
                        .HasForeignKey(ft => ft.UserID)
                        .IsRequired()
                );

            // Message N : 1 User (messages sent by an user)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserID)
                .IsRequired();

            // Message N : 1 PredictionGames (messages sent for a prediction game)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.PredictionGame)
                .WithMany(g => g.Messages)
                .HasForeignKey(m => m.PredictionGameID)
                .IsRequired(false);

            // Message N : 1 Match (messages sent for a match)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Match)
                .WithMany(m => m.Messages)
                .HasForeignKey(m => m.MatchID)
                .IsRequired(false);
        }

        private void SetData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemInformation>()
                .HasData(new SystemInformation
                {
                    ID = 1,
                    MatchesLastUpdateForCurrentDay = null
                });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SetTableNames(modelBuilder);
            SetColumns(modelBuilder);
            SetPrimaryKeys(modelBuilder);
            SetRelationships(modelBuilder);
            SetData(modelBuilder);
        }
    }
}
