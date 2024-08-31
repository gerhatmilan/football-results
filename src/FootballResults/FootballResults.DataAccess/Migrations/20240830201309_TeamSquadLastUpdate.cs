using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballResults.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TeamSquadLastUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "squad_last_update",
                schema: "football",
                table: "team",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "team_id",
                schema: "football",
                table: "player",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "squad_last_update",
                schema: "football",
                table: "team");

            migrationBuilder.AlterColumn<int>(
                name: "team_id",
                schema: "football",
                table: "player",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
