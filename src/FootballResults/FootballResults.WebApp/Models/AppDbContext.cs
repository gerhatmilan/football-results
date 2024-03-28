using Microsoft.EntityFrameworkCore;

namespace FootballResults.WebApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FavoriteLeague> FavoriteLeagues { get; set; }
        public DbSet<FavoriteTeam> FavoriteTeams { get; set; }

        private void SetTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("users");

            modelBuilder.Entity<Message>()
                .ToTable("messages");

            modelBuilder.Entity<FavoriteLeague>()
                .ToTable("favorite_leagues");

            modelBuilder.Entity<FavoriteTeam>()
                .ToTable("favorite_teams");
        }

        private void SetColumns(ModelBuilder modelBuilder)
        {
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
        }

        private void SetPrimaryKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            modelBuilder.Entity<Message>()
                .HasKey(m => m.MessageID);

            modelBuilder.Entity<FavoriteLeague>()
                .HasKey(fl => new { fl.UserID, fl.LeagueID });

            modelBuilder.Entity<FavoriteTeam>()
                .HasKey(ft => new { ft.UserID, ft.TeamID });
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

            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserID);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("users");

            SetTableNames(modelBuilder);
            SetColumns(modelBuilder);
            SetPrimaryKeys(modelBuilder);
            SetRelationShips(modelBuilder);
        }

        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is User && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((User)entityEntry.Entity).RegistrataionDate = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync();
        }
    }
}