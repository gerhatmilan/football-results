using FootballResults.Models.Users;
using FootballResults.Models.Predictions;
using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // users
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FavoriteLeague> FavoriteLeagues { get; set; }
        public DbSet<FavoriteTeam> FavoriteTeams { get; set; }

        // predictions
        public DbSet<PredictionGame> PredictionGames { get; set; }
        public DbSet<IncludedLeague> IncludedLeagues { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<GameStanding> Standings { get; set; }
        public DbSet<Participation> Participations { get; set; }


        private void SetTableNames(ModelBuilder modelBuilder)
        {
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
            modelBuilder.Entity<IncludedLeague>()
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
            modelBuilder.Entity<IncludedLeague>()
                .Property(il => il.GameID)
                .HasColumnName("prediction_game_id")
                .IsRequired(true);
            modelBuilder.Entity<IncludedLeague>()
                .Property(il => il.LeagueID)
                .HasColumnName("league_id")
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

            modelBuilder.Entity<IncludedLeague>()
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
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteLeagues)
                .WithOne(fl => fl.User)
                .HasForeignKey(fl => fl.UserID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteTeams)
                .WithOne(ft => ft.User)
                .HasForeignKey(ft => ft.UserID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserID);

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

            modelBuilder.Entity<User>()
                .HasMany(u => u.Predictions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Standings)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserID);

            modelBuilder.Entity<PredictionGame>()
                .HasMany(g => g.Predictions)
                .WithOne(p => p.Game)
                .HasForeignKey(p => p.GameID);

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