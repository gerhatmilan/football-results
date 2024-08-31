using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FootballResults.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PlayerUniqueIndexPlayerIDTeamID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_player",
                schema: "football",
                table: "player");

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "football",
                table: "player",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_player",
                schema: "football",
                table: "player",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_player_player_id_team_id",
                schema: "football",
                table: "player",
                columns: new[] { "player_id", "team_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_player",
                schema: "football",
                table: "player");

            migrationBuilder.DropIndex(
                name: "IX_player_player_id_team_id",
                schema: "football",
                table: "player");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "football",
                table: "player");

            migrationBuilder.AddPrimaryKey(
                name: "PK_player",
                schema: "football",
                table: "player",
                column: "player_id");
        }
    }
}
