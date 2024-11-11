using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FootballResults.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AdminUI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_admin",
                schema: "users",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "updates_active",
                schema: "football",
                table: "league",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "api_config",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    base_address = table.Column<string>(type: "varchar", nullable: false),
                    base_adress_header_key = table.Column<string>(type: "varchar", nullable: false),
                    api_key = table.Column<byte[]>(type: "bytea", nullable: true),
                    api_key_header_key = table.Column<string>(type: "varchar", nullable: false),
                    rate_limit = table.Column<int>(type: "int", nullable: true),
                    backup_data = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_api_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "application_config",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    update_worker_frequency = table.Column<long>(type: "bigint", nullable: false),
                    match_update_for_current_day_frequency = table.Column<long>(type: "bigint", nullable: false),
                    match_update_for_current_season_frequency = table.Column<long>(type: "bigint", nullable: false),
                    standings_update_for_current_season_frequency = table.Column<long>(type: "bigint", nullable: false),
                    top_scorers_update_for_current_season_frequency = table.Column<long>(type: "bigint", nullable: false),
                    image_download_worker_frequency = table.Column<long>(type: "bigint", nullable: false),
                    image_download_frequency = table.Column<long>(type: "bigint", nullable: false),
                    prediction_game_pictures_directory = table.Column<string>(type: "varchar", nullable: false),
                    profile_pictures_directory = table.Column<string>(type: "varchar", nullable: false),
                    countries_directory = table.Column<string>(type: "varchar", nullable: false),
                    leagues_directory = table.Column<string>(type: "varchar", nullable: false),
                    teams_directory = table.Column<string>(type: "varchar", nullable: false),
                    players_directory = table.Column<string>(type: "varchar", nullable: false),
                    prediction_game_default_image = table.Column<string>(type: "varchar", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "endpoint_config",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar", nullable: false),
                    endpoint = table.Column<string>(type: "varchar", nullable: false),
                    backup_path = table.Column<string>(type: "varchar", nullable: true),
                    load_data_from_backup = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_endpoint_config", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "api_config",
                columns: new[] { "id", "api_key", "api_key_header_key", "base_address", "base_adress_header_key", "rate_limit" },
                values: new object[] { 1, null, "x-rapidapi-key", "https://v3.football.api-sports.io", "x-rapidapi-host", 10 });

            migrationBuilder.InsertData(
                schema: "public",
                table: "application_config",
                columns: new[] { "id", "countries_directory", "image_download_frequency", "image_download_worker_frequency", "leagues_directory", "match_update_for_current_day_frequency", "match_update_for_current_season_frequency", "players_directory", "prediction_game_default_image", "prediction_game_pictures_directory", "profile_pictures_directory", "standings_update_for_current_season_frequency", "teams_directory", "top_scorers_update_for_current_season_frequency", "update_worker_frequency" },
                values: new object[] { 1, "images\\countries\\flags", 86400000000000L, 864000000000L, "images\\leagues\\logos", 6000000000L, 864000000000L, "images\\players", "images\\prediction-games\\default.jpg", "images\\prediction-games", "images\\profile-pictures", 864000000000L, "images\\teams\\logos", 864000000000L, 600000000L });

            migrationBuilder.InsertData(
                schema: "public",
                table: "endpoint_config",
                columns: new[] { "id", "backup_path", "endpoint", "name" },
                values: new object[,]
                {
                    { 1, ".\\databackup\\countries\\countries.json", "/countries", "Countries" },
                    { 2, ".\\databackup\\leagues\\leagues.json", "/leagues", "Leagues" },
                    { 3, ".\\databackup\\teams\\{0}\\{1}.json", "/teams?league={0}&season={1}", "TeamsForLeagueAndSeason" },
                    { 4, ".\\databackup\\squads\\{0}.json", "/players/squads?team={0}", "SquadForTeam" },
                    { 5, ".\\databackup\\matches\\{0}\\{1}.json", "/fixtures?league={0}&season={1}", "MatchesForLeagueAndSeason" },
                    { 6, ".\\databackup\\matches\\date\\{0}.json", "/fixtures?date={0}", "MatchesForDate" },
                    { 7, ".\\databackup\\standings\\{0}\\{1}.json", "/standings?league={0}&season={1}", "StandingsForLeagueAndSeason" },
                    { 8, ".\\databackup\\topscorers\\{0}\\{1}.json", "/players/topscorers?league={0}&season={1}", "TopScorersForLeagueAndSeason" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_endpoint_config_name",
                schema: "public",
                table: "endpoint_config",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "api_config",
                schema: "public");

            migrationBuilder.DropTable(
                name: "application_config",
                schema: "public");

            migrationBuilder.DropTable(
                name: "endpoint_config",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "is_admin",
                schema: "users",
                table: "user");

            migrationBuilder.DropColumn(
                name: "updates_active",
                schema: "football",
                table: "league");
        }
    }
}
