using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballResults.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SaveApiImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "photo_path",
                schema: "football",
                table: "topscorer",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "logo_path",
                schema: "football",
                table: "team",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "country_flags_last_download",
                schema: "public",
                table: "system_information",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "league_logos_last_download",
                schema: "public",
                table: "system_information",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "player_photos_last_download",
                schema: "public",
                table: "system_information",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "team_logos_last_download",
                schema: "public",
                table: "system_information",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "topscorer_photos_last_download",
                schema: "public",
                table: "system_information",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo_path",
                schema: "football",
                table: "player",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "logo_path",
                schema: "football",
                table: "league",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "flag_path",
                schema: "football",
                table: "country",
                type: "varchar",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "public",
                table: "system_information",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "country_flags_last_download", "league_logos_last_download", "player_photos_last_download", "team_logos_last_download", "topscorer_photos_last_download" },
                values: new object[] { null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "photo_path",
                schema: "football",
                table: "topscorer");

            migrationBuilder.DropColumn(
                name: "logo_path",
                schema: "football",
                table: "team");

            migrationBuilder.DropColumn(
                name: "country_flags_last_download",
                schema: "public",
                table: "system_information");

            migrationBuilder.DropColumn(
                name: "league_logos_last_download",
                schema: "public",
                table: "system_information");

            migrationBuilder.DropColumn(
                name: "player_photos_last_download",
                schema: "public",
                table: "system_information");

            migrationBuilder.DropColumn(
                name: "team_logos_last_download",
                schema: "public",
                table: "system_information");

            migrationBuilder.DropColumn(
                name: "topscorer_photos_last_download",
                schema: "public",
                table: "system_information");

            migrationBuilder.DropColumn(
                name: "photo_path",
                schema: "football",
                table: "player");

            migrationBuilder.DropColumn(
                name: "logo_path",
                schema: "football",
                table: "league");

            migrationBuilder.DropColumn(
                name: "flag_path",
                schema: "football",
                table: "country");
        }
    }
}
