using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballResults.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DefaultAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "users",
                table: "user",
                columns: new[] { "user_id", "email", "is_admin", "password", "profile_pic_path", "registration_date", "username" },
                values: new object[] { 1, "admin@admin.com", true, "AQAAAAIAAYagAAAAEEheRjhPc/l2VwT949G1nsqUy7Y7cayk7ohq5mxg90HvRB4/hss0fPMzAR9z6jGdSg==", null, new DateTime(2024, 11, 12, 23, 0, 10, 135, DateTimeKind.Unspecified).AddTicks(2324), "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "users",
                table: "user",
                keyColumn: "user_id",
                keyValue: 1);
        }
    }
}
