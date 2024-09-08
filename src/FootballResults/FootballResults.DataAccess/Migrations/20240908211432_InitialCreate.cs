using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FootballResults.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "football");

            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.EnsureSchema(
                name: "predictions");

            migrationBuilder.CreateTable(
                name: "country",
                schema: "football",
                columns: table => new
                {
                    country_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar", nullable: false),
                    flag_link = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_country", x => x.country_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "varchar", nullable: false),
                    username = table.Column<string>(type: "varchar", nullable: false),
                    password = table.Column<string>(type: "varchar", nullable: false),
                    profile_pic_path = table.Column<string>(type: "varchar", nullable: true),
                    registration_date = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "league",
                schema: "football",
                columns: table => new
                {
                    league_id = table.Column<int>(type: "int", nullable: false),
                    country_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "varchar", nullable: false),
                    type = table.Column<string>(type: "varchar", nullable: false),
                    logo_link = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league", x => x.league_id);
                    table.ForeignKey(
                        name: "FK_league_country_country_id",
                        column: x => x.country_id,
                        principalSchema: "football",
                        principalTable: "country",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "venue",
                schema: "football",
                columns: table => new
                {
                    venue_id = table.Column<int>(type: "int", nullable: false),
                    country_id = table.Column<int>(type: "int", nullable: false),
                    city = table.Column<string>(type: "varchar", nullable: true),
                    name = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venue", x => x.venue_id);
                    table.ForeignKey(
                        name: "FK_venue_country_country_id",
                        column: x => x.country_id,
                        principalSchema: "football",
                        principalTable: "country",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prediction_game",
                schema: "predictions",
                columns: table => new
                {
                    prediction_game_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar", nullable: false),
                    join_key = table.Column<string>(type: "varchar", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    image_path = table.Column<string>(type: "varchar", nullable: true),
                    exact_scoreline_reward = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    outcome_reward = table.Column<int>(type: "int", nullable: false, defaultValue: 8),
                    goal_count_reward = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    goal_difference_reward = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    created_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    standings_last_update = table.Column<DateTime>(type: "timestamp", nullable: true),
                    finished = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prediction_game", x => x.prediction_game_id);
                    table.ForeignKey(
                        name: "FK_prediction_game_user_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "users",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorite_league",
                schema: "users",
                columns: table => new
                {
                    favorite_league_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    league_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorite_league", x => x.favorite_league_id);
                    table.ForeignKey(
                        name: "FK_favorite_league_league_league_id",
                        column: x => x.league_id,
                        principalSchema: "football",
                        principalTable: "league",
                        principalColumn: "league_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favorite_league_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "league_season",
                schema: "football",
                columns: table => new
                {
                    league_season_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    league_id = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    in_progress = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    standings_last_update = table.Column<DateTime>(type: "timestamp", nullable: true),
                    topscorers_last_update = table.Column<DateTime>(type: "timestamp", nullable: true),
                    matches_last_update = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league_season", x => x.league_season_id);
                    table.ForeignKey(
                        name: "FK_league_season_league_league_id",
                        column: x => x.league_id,
                        principalSchema: "football",
                        principalTable: "league",
                        principalColumn: "league_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team",
                schema: "football",
                columns: table => new
                {
                    team_id = table.Column<int>(type: "int", nullable: false),
                    country_id = table.Column<int>(type: "int", nullable: false),
                    venue_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "varchar", nullable: false),
                    short_name = table.Column<string>(type: "varchar", nullable: true),
                    logo_link = table.Column<string>(type: "varchar", nullable: true),
                    national = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    squad_last_update = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team", x => x.team_id);
                    table.ForeignKey(
                        name: "FK_team_country_country_id",
                        column: x => x.country_id,
                        principalSchema: "football",
                        principalTable: "country",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_team_venue_venue_id",
                        column: x => x.venue_id,
                        principalSchema: "football",
                        principalTable: "venue",
                        principalColumn: "venue_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "participation",
                schema: "predictions",
                columns: table => new
                {
                    participation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prediction_game_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    standing_id = table.Column<int>(type: "int", nullable: false),
                    join_date = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_participation", x => x.participation_id);
                    table.ForeignKey(
                        name: "FK_participation_prediction_game_prediction_game_id",
                        column: x => x.prediction_game_id,
                        principalSchema: "predictions",
                        principalTable: "prediction_game",
                        principalColumn: "prediction_game_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_participation_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prediction_game_season",
                schema: "predictions",
                columns: table => new
                {
                    predicton_game_season_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    prediction_game_id = table.Column<int>(type: "int", nullable: false),
                    league_season_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prediction_game_season", x => x.predicton_game_season_id);
                    table.ForeignKey(
                        name: "FK_prediction_game_season_league_season_league_season_id",
                        column: x => x.league_season_id,
                        principalSchema: "football",
                        principalTable: "league_season",
                        principalColumn: "league_season_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prediction_game_season_prediction_game_prediction_game_id",
                        column: x => x.prediction_game_id,
                        principalSchema: "predictions",
                        principalTable: "prediction_game",
                        principalColumn: "prediction_game_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "favorite_team",
                schema: "users",
                columns: table => new
                {
                    favorite_team_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    team_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorite_team", x => x.favorite_team_id);
                    table.ForeignKey(
                        name: "FK_favorite_team_team_team_id",
                        column: x => x.team_id,
                        principalSchema: "football",
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favorite_team_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match",
                schema: "football",
                columns: table => new
                {
                    match_id = table.Column<int>(type: "int", nullable: false),
                    league_season_id = table.Column<int>(type: "int", nullable: false),
                    venue_id = table.Column<int>(type: "int", nullable: true),
                    home_team_id = table.Column<int>(type: "int", nullable: false),
                    away_team_id = table.Column<int>(type: "int", nullable: false),
                    round = table.Column<string>(type: "varchar", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp", nullable: true),
                    status = table.Column<string>(type: "varchar", nullable: true),
                    minute = table.Column<int>(type: "int", nullable: true),
                    home_team_goals = table.Column<int>(type: "int", nullable: true),
                    away_team_goals = table.Column<int>(type: "int", nullable: true),
                    last_update = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match", x => x.match_id);
                    table.ForeignKey(
                        name: "FK_match_league_season_league_season_id",
                        column: x => x.league_season_id,
                        principalSchema: "football",
                        principalTable: "league_season",
                        principalColumn: "league_season_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_team_away_team_id",
                        column: x => x.away_team_id,
                        principalSchema: "football",
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_team_home_team_id",
                        column: x => x.home_team_id,
                        principalSchema: "football",
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_match_venue_venue_id",
                        column: x => x.venue_id,
                        principalSchema: "football",
                        principalTable: "venue",
                        principalColumn: "venue_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "player",
                schema: "football",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "int", nullable: false),
                    team_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "varchar", nullable: false),
                    age = table.Column<int>(type: "int", nullable: true),
                    number = table.Column<int>(type: "int", nullable: true),
                    position = table.Column<string>(type: "varchar", nullable: true),
                    photo_link = table.Column<string>(type: "varchar", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player", x => x.id);
                    table.ForeignKey(
                        name: "FK_player_team_team_id",
                        column: x => x.team_id,
                        principalSchema: "football",
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "standing",
                schema: "football",
                columns: table => new
                {
                    standing_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    league_season_id = table.Column<int>(type: "int", nullable: false),
                    team_id = table.Column<int>(type: "int", nullable: false),
                    rank = table.Column<int>(type: "int", nullable: false),
                    group = table.Column<string>(type: "varchar", nullable: true),
                    points = table.Column<int>(type: "int", nullable: false),
                    played = table.Column<int>(type: "int", nullable: false),
                    wins = table.Column<int>(type: "int", nullable: false),
                    draws = table.Column<int>(type: "int", nullable: false),
                    losses = table.Column<int>(type: "int", nullable: false),
                    scored = table.Column<int>(type: "int", nullable: false),
                    conceded = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_standing", x => x.standing_id);
                    table.ForeignKey(
                        name: "FK_standing_league_season_league_season_id",
                        column: x => x.league_season_id,
                        principalSchema: "football",
                        principalTable: "league_season",
                        principalColumn: "league_season_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_standing_team_team_id",
                        column: x => x.team_id,
                        principalSchema: "football",
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "topscorer",
                schema: "football",
                columns: table => new
                {
                    topscorer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    league_season_id = table.Column<int>(type: "int", nullable: false),
                    team_id = table.Column<int>(type: "int", nullable: false),
                    rank = table.Column<int>(type: "int", nullable: false),
                    player_name = table.Column<string>(type: "varchar", nullable: false),
                    photo_link = table.Column<string>(type: "varchar", nullable: true),
                    played = table.Column<int>(type: "int", nullable: true),
                    goals = table.Column<int>(type: "int", nullable: false),
                    assists = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topscorer", x => x.topscorer_id);
                    table.ForeignKey(
                        name: "FK_topscorer_league_season_league_season_id",
                        column: x => x.league_season_id,
                        principalSchema: "football",
                        principalTable: "league_season",
                        principalColumn: "league_season_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_topscorer_team_team_id",
                        column: x => x.team_id,
                        principalSchema: "football",
                        principalTable: "team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "standing",
                schema: "predictions",
                columns: table => new
                {
                    standing_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    participation_id = table.Column<int>(type: "int", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_standing", x => x.standing_id);
                    table.ForeignKey(
                        name: "FK_standing_participation_participation_id",
                        column: x => x.participation_id,
                        principalSchema: "predictions",
                        principalTable: "participation",
                        principalColumn: "participation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message",
                schema: "users",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<int>(type: "int", nullable: true),
                    prediction_game_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp", nullable: true),
                    text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_message_match_match_id",
                        column: x => x.match_id,
                        principalSchema: "football",
                        principalTable: "match",
                        principalColumn: "match_id");
                    table.ForeignKey(
                        name: "FK_message_prediction_game_prediction_game_id",
                        column: x => x.prediction_game_id,
                        principalSchema: "predictions",
                        principalTable: "prediction_game",
                        principalColumn: "prediction_game_id");
                    table.ForeignKey(
                        name: "FK_message_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "users",
                        principalTable: "user",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prediction",
                schema: "predictions",
                columns: table => new
                {
                    prediction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    participation_id = table.Column<int>(type: "int", nullable: false),
                    match_id = table.Column<int>(type: "int", nullable: false),
                    home_team_goals = table.Column<int>(type: "int", nullable: false),
                    away_team_goals = table.Column<int>(type: "int", nullable: false),
                    points_given = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    prediction_date = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prediction", x => x.prediction_id);
                    table.ForeignKey(
                        name: "FK_prediction_match_match_id",
                        column: x => x.match_id,
                        principalSchema: "football",
                        principalTable: "match",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_prediction_participation_participation_id",
                        column: x => x.participation_id,
                        principalSchema: "predictions",
                        principalTable: "participation",
                        principalColumn: "participation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_country_name",
                schema: "football",
                table: "country",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_favorite_league_league_id",
                schema: "users",
                table: "favorite_league",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_league_user_id_league_id",
                schema: "users",
                table: "favorite_league",
                columns: new[] { "user_id", "league_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_favorite_team_team_id",
                schema: "users",
                table: "favorite_team",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_team_user_id_team_id",
                schema: "users",
                table: "favorite_team",
                columns: new[] { "user_id", "team_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_league_country_id",
                schema: "football",
                table: "league",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_league_season_league_id",
                schema: "football",
                table: "league_season",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_away_team_id",
                schema: "football",
                table: "match",
                column: "away_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_home_team_id",
                schema: "football",
                table: "match",
                column: "home_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_league_season_id",
                schema: "football",
                table: "match",
                column: "league_season_id");

            migrationBuilder.CreateIndex(
                name: "IX_match_venue_id",
                schema: "football",
                table: "match",
                column: "venue_id");

            migrationBuilder.CreateIndex(
                name: "IX_message_match_id",
                schema: "users",
                table: "message",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_message_prediction_game_id",
                schema: "users",
                table: "message",
                column: "prediction_game_id");

            migrationBuilder.CreateIndex(
                name: "IX_message_user_id",
                schema: "users",
                table: "message",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_participation_prediction_game_id_user_id",
                schema: "predictions",
                table: "participation",
                columns: new[] { "prediction_game_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_participation_user_id",
                schema: "predictions",
                table: "participation",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_player_player_id_team_id",
                schema: "football",
                table: "player",
                columns: new[] { "player_id", "team_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_player_team_id",
                schema: "football",
                table: "player",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_prediction_match_id",
                schema: "predictions",
                table: "prediction",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_prediction_participation_id_match_id",
                schema: "predictions",
                table: "prediction",
                columns: new[] { "participation_id", "match_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prediction_game_join_key",
                schema: "predictions",
                table: "prediction_game",
                column: "join_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prediction_game_owner_id",
                schema: "predictions",
                table: "prediction_game",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_prediction_game_season_league_season_id",
                schema: "predictions",
                table: "prediction_game_season",
                column: "league_season_id");

            migrationBuilder.CreateIndex(
                name: "IX_prediction_game_season_prediction_game_id_league_season_id",
                schema: "predictions",
                table: "prediction_game_season",
                columns: new[] { "prediction_game_id", "league_season_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_standing_league_season_id_group_team_id",
                schema: "football",
                table: "standing",
                columns: new[] { "league_season_id", "group", "team_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_standing_team_id",
                schema: "football",
                table: "standing",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_standing_participation_id",
                schema: "predictions",
                table: "standing",
                column: "participation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_team_country_id",
                schema: "football",
                table: "team",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_venue_id",
                schema: "football",
                table: "team",
                column: "venue_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                schema: "users",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_username",
                schema: "users",
                table: "user",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_venue_country_id",
                schema: "football",
                table: "venue",
                column: "country_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorite_league",
                schema: "users");

            migrationBuilder.DropTable(
                name: "favorite_team",
                schema: "users");

            migrationBuilder.DropTable(
                name: "message",
                schema: "users");

            migrationBuilder.DropTable(
                name: "player",
                schema: "football");

            migrationBuilder.DropTable(
                name: "prediction",
                schema: "predictions");

            migrationBuilder.DropTable(
                name: "prediction_game_season",
                schema: "predictions");

            migrationBuilder.DropTable(
                name: "standing",
                schema: "football");

            migrationBuilder.DropTable(
                name: "standing",
                schema: "predictions");

            migrationBuilder.DropTable(
                name: "topscorer",
                schema: "football");

            migrationBuilder.DropTable(
                name: "match",
                schema: "football");

            migrationBuilder.DropTable(
                name: "participation",
                schema: "predictions");

            migrationBuilder.DropTable(
                name: "league_season",
                schema: "football");

            migrationBuilder.DropTable(
                name: "team",
                schema: "football");

            migrationBuilder.DropTable(
                name: "prediction_game",
                schema: "predictions");

            migrationBuilder.DropTable(
                name: "league",
                schema: "football");

            migrationBuilder.DropTable(
                name: "venue",
                schema: "football");

            migrationBuilder.DropTable(
                name: "user",
                schema: "users");

            migrationBuilder.DropTable(
                name: "country",
                schema: "football");
        }
    }
}
