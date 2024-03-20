using Microsoft.EntityFrameworkCore;
using FootballResults.Models;

namespace FootballResults.API.Models
{
    public class FootballDataDbContext : DbContext
    {
        public FootballDataDbContext(DbContextOptions<FootballDataDbContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<AvailableSeason> AvailableSeasons { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Standing> Standings { get; set; }
        public DbSet<TopScorer> TopScorers { get; set; }
        public DbSet<Player> Players { get; set; }

        private void SetTableNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .ToTable("countries");

            modelBuilder.Entity<League>()
                .ToTable("leagues");

            modelBuilder.Entity<Team>()
                .ToTable("teams");

            modelBuilder.Entity<Venue>()
                .ToTable("venues");

            modelBuilder.Entity<Match>()
                .ToTable("matches");

            modelBuilder.Entity<AvailableSeason>()
                .ToTable("available_seasons");

            modelBuilder.Entity<Standing>()
                .ToTable("standings");

            modelBuilder.Entity<TopScorer>()
                .ToTable("top_scorers");

            modelBuilder.Entity<Player>()
                .ToTable("players");
        }

        private void SetColumns(ModelBuilder modelBuilder)
        {
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
                .IsRequired(false);
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
            modelBuilder.Entity<Standing>()
                .Property(s => s.LeagueID)
                .HasColumnName("league_id")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Season)
                .HasColumnName("season")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.TeamID)
                .HasColumnName("team_id")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Rank)
                .HasColumnName("rank")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Group)
                .HasColumnName("group")
                .IsRequired(false);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Points)
                .HasColumnName("points")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Played)
                .HasColumnName("played")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Wins)
                .HasColumnName("wins")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Draws)
                .HasColumnName("draws")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Losses)
                .HasColumnName("losses")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Scored)
                .HasColumnName("scored")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
                .Property(s => s.Conceded)
                .HasColumnName("conceded")
                .IsRequired(true);
            modelBuilder.Entity<Standing>()
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
        }

        private void SetPrimaryKeys(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Standing>()
                .HasKey(s => new { s.LeagueID, s.Season, s.TeamID });

            modelBuilder.Entity<TopScorer>()
                .HasKey(ts => new { ts.LeagueID, ts.Season, ts.Rank });

            modelBuilder.Entity<Player>()
                .HasKey(p => new { p.PlayerID, p.TeamID });
        }

        private void SetRelationShips(ModelBuilder modelBuilder)
        {
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
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("football");

            SetTableNames(modelBuilder);
            SetColumns(modelBuilder);
            SetPrimaryKeys(modelBuilder);
            SetRelationShips(modelBuilder);
        }
    }
}
