using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Models
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
                .HasMany(u => u.PredictionGames)
                .WithOne(g => g.Owner)
                .HasForeignKey(g => g.OwnerID);
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
                .Where(e => e.Entity is User && (
                    e.State == EntityState.Added));

            var messageEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Message && (
                    e.State == EntityState.Added));

            var predictionGameEntries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is PredictionGame && (
                    e.State == EntityState.Added));

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

            return await base.SaveChangesAsync();
        }
    }
}