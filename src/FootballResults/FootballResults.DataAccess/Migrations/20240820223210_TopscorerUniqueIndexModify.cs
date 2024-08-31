using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballResults.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TopscorerUniqueIndexModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_topscorer_league_season_id",
                schema: "football",
                table: "topscorer");

            migrationBuilder.DropIndex(
                name: "IX_topscorer_team_id_rank",
                schema: "football",
                table: "topscorer");

            migrationBuilder.CreateIndex(
                name: "IX_topscorer_league_season_id_rank",
                schema: "football",
                table: "topscorer",
                columns: new[] { "league_season_id", "rank" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_topscorer_team_id",
                schema: "football",
                table: "topscorer",
                column: "team_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_topscorer_league_season_id_rank",
                schema: "football",
                table: "topscorer");

            migrationBuilder.DropIndex(
                name: "IX_topscorer_team_id",
                schema: "football",
                table: "topscorer");

            migrationBuilder.CreateIndex(
                name: "IX_topscorer_league_season_id",
                schema: "football",
                table: "topscorer",
                column: "league_season_id");

            migrationBuilder.CreateIndex(
                name: "IX_topscorer_team_id_rank",
                schema: "football",
                table: "topscorer",
                columns: new[] { "team_id", "rank" },
                unique: true);
        }
    }
}
