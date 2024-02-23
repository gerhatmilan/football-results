CREATE SCHEMA "football";

CREATE TABLE "football"."countries" (
  "country_id" varchar PRIMARY KEY,
  "flag_link" varchar
);

CREATE TABLE "football"."leagues" (
  "league_id" integer PRIMARY KEY,
  "country_id" varchar,
  "name" varchar UNIQUE NOT NULL,
  "type" varchar NOT NULL CHECK("football"."leagues"."type" IN ('league', 'cup')),
  "current_season" integer NOT NULL,
  "logo_link" varchar,
  
  FOREIGN KEY ("country_id")
    REFERENCES "football"."countries" ("country_id")
	ON DELETE CASCADE
);

CREATE TABLE "football"."venues" (
  "venue_id" integer PRIMARY KEY,
  "country_id" varchar NOT NULL,
  "city" varchar NOT NULL,
  "name" varchar UNIQUE NOT NULL,
  
  FOREIGN KEY ("country_id")
    REFERENCES "football"."countries" ("country_id")
	ON DELETE CASCADE
);

CREATE TABLE "football"."teams" (
  "team_id" integer PRIMARY KEY,
  "country_id" varchar NOT NULL,
  "venue_id" integer,
  "name" varchar UNIQUE NOT NULL,
  "short_name" varchar,
  "logo_link" varchar,
  "national" boolean NOT NULL DEFAULT false,
  
  FOREIGN KEY ("country_id")
    REFERENCES "football"."countries" ("country_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("venue_id")
    REFERENCES "football"."venues" ("venue_id")
	ON DELETE SET NULL
);

CREATE TABLE "football"."available_seasons" (
  "league_id" integer,
  "season" integer,
  
  PRIMARY KEY ("league_id", "season"),
  
  FOREIGN KEY ("league_id")
    REFERENCES "football"."leagues" ("league_id")
	ON DELETE CASCADE
);

CREATE TABLE "football"."standings" (
  "league_id" integer,
  "season" integer,
  "rank" integer,
  "team_id" integer,
  "points" integer NOT NULL,
  "played" integer NOT NULL,
  "wins" integer NOT NULL,
  "draws" integer NOT NULL,
  "losses" integer NOT NULL,
  "scored" integer NOT NULL,
  "condeded" integer NOT NULL,
  "last_update" timestamp,
  
  PRIMARY KEY ("league_id", "season", "rank"),
  
  FOREIGN KEY ("league_id", "season")
    REFERENCES "football"."available_seasons" ("league_id", "season")
	ON DELETE CASCADE,
  FOREIGN KEY ("team_id")
    REFERENCES "football"."teams" ("team_id")
	ON DELETE SET NULL
);

CREATE TABLE "football"."players" (
  "player_id" integer PRIMARY KEY,
  "team_id" integer,
  "name" varchar NOT NULL,
  "age" integer NOT NULL,
  "number" integer,
  "position" varchar,
  "photo_link" varchar,
  
  FOREIGN KEY ("team_id")
    REFERENCES "football"."teams" ("team_id")
	ON DELETE SET NULL
);

CREATE TABLE "football"."top_scorers" (
  "league_id" integer,
  "season" integer,
  "rank" integer,
  "player_id" integer,
  "played" integer NOT NULL,
  "goals" integer NOT NULL,
  "assists" integer NOT NULL,
  "last_update" timestamp,
  
  PRIMARY KEY ("league_id", "season", "rank"),
  
  FOREIGN KEY ("league_id", "season")
    REFERENCES "football"."available_seasons" ("league_id", "season")
	ON DELETE CASCADE,
  FOREIGN KEY ("player_id")
    REFERENCES "football"."players" ("player_id")
	ON DELETE SET NULL
);

CREATE TABLE "football"."matches" (
  "match_id" integer PRIMARY KEY,
  "date" timestamp,
  "venue_id" integer,
  "league_id" integer NOT NULL,
  "season" integer NOT NULL,
  "round" integer NOT NULL,
  "home_team_id" integer NOT NULL,
  "away_team_id" integer NOT NULL,
  "status" varchar,
  "minute" integer,
  "home_team_goals" integer,
  "away_team_goals" integer,
  "last_update" timestamp,
  
  FOREIGN KEY ("venue_id")
    REFERENCES "football"."venues" ("venue_id")
	ON DELETE SET NULL,
  FOREIGN KEY ("league_id", "season")
    REFERENCES "football"."available_seasons" ("league_id", "season")
	ON DELETE CASCADE,
  FOREIGN KEY ("home_team_id")
    REFERENCES "football"."teams" ("team_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("away_team_id")
    REFERENCES "football"."teams" ("team_id")
	ON DELETE CASCADE
);
