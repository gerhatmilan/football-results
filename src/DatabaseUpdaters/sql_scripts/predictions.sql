CREATE SCHEMA "predictions";

CREATE TABLE "predictions"."prediction_games" (
  "prediction_game_id" serial PRIMARY KEY,
  "name" varchar NOT NULL,
  "owner_id" integer NOT NULL,
  "join_key" varchar UNIQUE NOT NULL,
  "description" text,
  "image_path" varchar,
  "exact_scoreline_reward" integer NOT NULL DEFAULT 10 CHECK("exact_scoreline_reward" > 0),
  "outcome_reward" integer NOT NULL DEFAULT 8 CHECK("outcome_reward" > 0),
  "goal_count_reward" integer NOT NULL DEFAULT 5 CHECK("goal_count_reward" > 0),
  "goal_difference_reward" integer NOT NULL DEFAULT 3 CHECK("goal_difference_reward" > 0),
  "created_at" timestamptz NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "finished" boolean NOT NULL DEFAULT false,
  
  FOREIGN KEY ("owner_id")
    REFERENCES "users"."users" ("user_id")
	ON DELETE CASCADE
);

CREATE TABLE "predictions"."included_leagues" (
  "prediction_game_id" integer NOT NULL,
  "league_id" integer NOT NULL,
  
  PRIMARY KEY (prediction_game_id, league_id),
  
  FOREIGN KEY ("prediction_game_id")
    REFERENCES "predictions"."prediction_games" ("prediction_game_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("league_id")
    REFERENCES "football"."leagues" ("league_id")
	ON DELETE CASCADE
);

CREATE TABLE "predictions"."predictions" (
  "user_id" integer,
  "prediction_game_id" integer,
  "match_id" integer,
  "home_team_goals" integer NOT NULL,
  "away_team_goals" integer NOT NULL,
  "prediction_date" timestamptz DEFAULT CURRENT_TIMESTAMP,
  
  PRIMARY KEY ("user_id", "prediction_game_id", "match_id"),
	
  FOREIGN KEY ("user_id")
    REFERENCES "users"."users" ("user_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("prediction_game_id")
    REFERENCES "predictions"."prediction_games" ("prediction_game_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("match_id")
    REFERENCES "football"."matches" ("match_id")
	ON DELETE CASCADE
);

CREATE TABLE "predictions"."standings" (
  "prediction_game_id" integer NOT NULL,
  "user_id" integer NOT NULL,
  "points" integer NOT NULL,
  "last_update" timestamptz DEFAULT CURRENT_TIMESTAMP,
  
  PRIMARY KEY (prediction_game_id, user_id),
  
  FOREIGN KEY ("user_id")
    REFERENCES "users"."users" ("user_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("prediction_game_id")
    REFERENCES "predictions"."prediction_games" ("prediction_game_id")
	ON DELETE CASCADE
);

CREATE TABLE "predictions"."participations" (
  "prediction_game_id" integer NOT NULL,
  "user_id" integer NOT NULL,
  "join_date" timestamptz DEFAULT CURRENT_TIMESTAMP,
	
  PRIMARY KEY (prediction_game_id, user_id),
	
  FOREIGN KEY ("prediction_game_id")
    REFERENCES "predictions"."prediction_games" ("prediction_game_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("user_id")
    REFERENCES "users"."users" ("user_id")
	ON DELETE CASCADE
);


-- user must participate in game to predict

-- predictions.match_id "constraint": the league's id in which this match takes place
-- must be in one of the leagues which is part of this prediction game

CREATE OR REPLACE FUNCTION "predictions".validate_prediction()
RETURNS TRIGGER AS $$
DECLARE
  league_id_of_match integer;
BEGIN
  SELECT "league_id" INTO league_id_of_match FROM "football"."matches" WHERE "match_id" = NEW."match_id" LIMIT 1;

  IF league_id_of_match NOT IN (SELECT "league_id" FROM "predictions"."included_leagues" WHERE "prediction_game_id" = NEW."prediction_game_id") THEN
    RAISE EXCEPTION 'Invalid prediction: The match is not included in this prediction game';
  ELSIF NEW."user_id" NOT IN (SELECT "user_id" FROM "predictions"."participations" WHERE "prediction_game_id" = NEW."prediction_game_id") THEN
  	RAISE EXCEPTION 'Invalid prediction: User must join the game to predict';
  END IF;

  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER validate_prediction_trigger
BEFORE INSERT ON "predictions"."predictions"
FOR EACH ROW EXECUTE FUNCTION "predictions".validate_prediction();


-- create participants entry when a user creates a new game

CREATE OR REPLACE FUNCTION "predictions".add_owner_as_participant()
RETURNS TRIGGER AS $$
BEGIN
	INSERT INTO "predictions"."participations" (prediction_game_id, user_id, join_date)
	VALUES (
		NEW."prediction_game_id",
		NEW."owner_id",
		now()
	);

	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER new_game_trigger
AFTER INSERT ON "predictions"."prediction_games"
FOR EACH ROW EXECUTE FUNCTION "predictions".add_owner_as_participant();


-- create a standings entry when user joins a game

CREATE OR REPLACE FUNCTION "predictions".add_standings_entry()
RETURNS TRIGGER AS $$
BEGIN
	INSERT INTO "predictions"."standings" (prediction_game_id, user_id, points, last_update)
	VALUES (
		NEW."prediction_game_id",
		NEW."user_id",
		0,
		now()
	);

	RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER after_new_participant_trigger
AFTER INSERT ON "predictions"."participations"
FOR EACH ROW EXECUTE FUNCTION "predictions".add_standings_entry();