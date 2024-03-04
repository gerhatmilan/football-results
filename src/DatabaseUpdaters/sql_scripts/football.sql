CREATE SCHEMA "football";

CREATE TABLE "football"."countries" (
  "country_id" varchar PRIMARY KEY,
  "flag_link" varchar
);

CREATE TABLE "football"."leagues" (
  "league_id" integer PRIMARY KEY,
  "country_id" varchar,
  "name" varchar NOT NULL,
  "type" varchar NOT NULL CHECK("football"."leagues"."type" IN ('League', 'Cup')),
  "current_season" integer NOT NULL,
  "logo_link" varchar,
  
  FOREIGN KEY ("country_id")
    REFERENCES "football"."countries" ("country_id")
	ON DELETE CASCADE
);

CREATE TABLE "football"."venues" (
  "venue_id" integer PRIMARY KEY,
  "country_id" varchar NOT NULL,
  "city" varchar,
  "name" varchar,
  
  FOREIGN KEY ("country_id")
    REFERENCES "football"."countries" ("country_id")
	ON DELETE CASCADE
);

CREATE TABLE "football"."teams" (
  "team_id" integer PRIMARY KEY,
  "country_id" varchar NOT NULL,
  "venue_id" integer,
  "name" varchar NOT NULL,
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
  "team_id" integer,
  "rank" integer NOT NULL,
  "group" varchar,
  "points" integer NOT NULL,
  "played" integer NOT NULL,
  "wins" integer NOT NULL,
  "draws" integer NOT NULL,
  "losses" integer NOT NULL,
  "scored" integer NOT NULL,
  "conceded" integer NOT NULL,
  "last_update" timestamp DEFAULT CURRENT_TIMESTAMP,
  
  PRIMARY KEY ("league_id", "season", "team_id"),
  
  FOREIGN KEY ("league_id", "season")
    REFERENCES "football"."available_seasons" ("league_id", "season")
	ON DELETE CASCADE,
  FOREIGN KEY ("team_id")
    REFERENCES "football"."teams" ("team_id")
	ON DELETE SET NULL
);

CREATE TABLE "football"."players" (
  "player_id" integer,
  "team_id" integer,
  "name" varchar NOT NULL,
  "age" integer,
  "number" integer,
  "position" varchar,
  "photo_link" varchar,
  
  PRIMARY KEY("player_id", "team_id"),
  
  FOREIGN KEY ("team_id")
    REFERENCES "football"."teams" ("team_id")
	ON DELETE SET NULL
);

CREATE TABLE "football"."top_scorers" (
  "league_id" integer,
  "season" integer,
  "rank" integer NOT NULL,
  "player_name" varchar NOT NULL,
  "photo_link" varchar,
  "team_id" integer NOT NULL,
  "played" integer,
  "goals" integer NOT NULL,
  "assists" integer,
  "last_update" timestamp DEFAULT CURRENT_TIMESTAMP,
  
  PRIMARY KEY ("league_id", "season", "rank"),
  
  FOREIGN KEY ("league_id", "season")
    REFERENCES "football"."available_seasons" ("league_id", "season")
	ON DELETE CASCADE,
	
  FOREIGN KEY ("team_id")
    REFERENCES "football"."teams" ("team_id")
	ON DELETE CASCADE
);

CREATE TABLE "football"."matches" (
  "match_id" integer PRIMARY KEY,
  "date" timestamp,
  "venue_id" integer,
  "league_id" integer NOT NULL,
  "season" integer NOT NULL,
  "round" varchar,
  "home_team_id" integer NOT NULL,
  "away_team_id" integer NOT NULL,
  "status" varchar,
  "minute" integer,
  "home_team_goals" integer,
  "away_team_goals" integer,
  "last_update" timestamp DEFAULT CURRENT_TIMESTAMP,
  
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



-- ensures that only group phase records are inserted

CREATE OR REPLACE FUNCTION "football".drop_if_qualification()
RETURNS TRIGGER AS $$
BEGIN
	IF NEW."group" NOT LIKE 'GROUP _' AND NEW."group" NOT LIKE 'Ranking of%' THEN
		RETURN NEW;
	ELSE
		RAISE EXCEPTION 'No need to insert qualification data.';
	END IF;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER before_new_standings_trigger
BEFORE INSERT OR UPDATE ON "football"."standings"
FOR EACH ROW EXECUTE FUNCTION "football".drop_if_qualification();



-- create short name where it is null by default with manually calling this function

CREATE OR REPLACE FUNCTION football.resolve_short_names_with_null()
RETURNS VOID AS $$
DECLARE
    row_data RECORD;
	updates INTEGER = 0;
BEGIN
    FOR row_data IN SELECT * FROM football.teams
    LOOP
    	IF row_data.short_name IS NULL THEN
			RAISE NOTICE 'Updating short_name for team %', row_data.name;
			UPDATE football.teams
			SET short_name = UPPER(SUBSTRING(name FROM 1 FOR 3))
			WHERE team_id = row_data.team_id;
			RAISE NOTICE 'Update complete for %s, new short_name: %', row_data.name, row_data.short_name;
    		updates := updates + 1;
		END IF;
	END LOOP;
	RAISE NOTICE 'Total updates: %', updates;

END;
$$ LANGUAGE plpgsql;



-- trigger for creating short_name automatically

CREATE OR REPLACE FUNCTION football.create_short_name_if_null()
RETURNS TRIGGER AS $$
BEGIN
	IF NEW.short_name IS NULL THEN
		NEW.short_name := UPPER(SUBSTRING(NEW.name FROM 1 FOR 3));
	END IF;
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER before_insert_or_update_on_teams
BEFORE INSERT OR UPDATE ON "football"."teams"
FOR EACH ROW EXECUTE FUNCTION "football".create_short_name_if_null();


-- triggers for updating the last_update fields of tables

CREATE OR REPLACE FUNCTION football.refresh_last_update_field()
RETURNS TRIGGER AS $$
BEGIN
	NEW.last_update := now();
	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER after_update_on_matches
AFTER UPDATE ON "football"."matches"
FOR EACH ROW EXECUTE FUNCTION "football".refresh_last_update_field();

CREATE OR REPLACE TRIGGER after_update_on_standings
AFTER UPDATE ON "football"."standings"
FOR EACH ROW EXECUTE FUNCTION "football".refresh_last_update_field();

CREATE OR REPLACE TRIGGER after_update_on_top_scorers
AFTER UPDATE ON "football"."top_scorers"
FOR EACH ROW EXECUTE FUNCTION "football".refresh_last_update_field();