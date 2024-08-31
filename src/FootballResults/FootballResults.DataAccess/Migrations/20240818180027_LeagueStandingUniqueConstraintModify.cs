using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballResults.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class LeagueStandingUniqueConstraintModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_standing_league_season_id_team_id",
                schema: "football",
                table: "standing");

            migrationBuilder.CreateIndex(
                name: "IX_standing_league_season_id_group_team_id",
                schema: "football",
                table: "standing",
                columns: new[] { "league_season_id", "group", "team_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_standing_league_season_id_group_team_id",
                schema: "football",
                table: "standing");

            migrationBuilder.CreateIndex(
                name: "IX_standing_league_season_id_team_id",
                schema: "football",
                table: "standing",
                columns: new[] { "league_season_id", "team_id" },
                unique: true);
        }
    }
}
