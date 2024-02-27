CREATE SCHEMA "users";

CREATE TABLE "users"."users" (
  "user_id" serial PRIMARY KEY,
  "email" varchar UNIQUE NOT NULL,
  "username" varchar UNIQUE NOT NULL,
  "password" bytea NOT NULL,
  "registration_date" date NOT NULL DEFAULT CURRENT_DATE
);

CREATE TABLE "users"."messages" (
  "message_id" serial PRIMARY KEY,
  "user_id" integer NOT NULL,
  "sent_at" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "message" text,
  "match_id" integer,
  "prediction_game_id" integer,
  
  FOREIGN KEY ("user_id")
    REFERENCES "users"."users" ("user_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("match_id")
    REFERENCES "football"."matches" ("match_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("prediction_game_id")
    REFERENCES "predictions"."prediction_games" ("prediction_game_id")
	ON DELETE CASCADE
);

CREATE TABLE "users"."favorite_leagues" (
  "user_id" integer NOT NULL,
  "league_id" integer NOT NULL,
  
  PRIMARY KEY ("user_id", "league_id"),
  
  FOREIGN KEY ("user_id")
    REFERENCES "users"."users" ("user_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("league_id")
    REFERENCES "football"."leagues" ("league_id")
	ON DELETE CASCADE
);

CREATE TABLE "users"."favorite_teams" (
  "user_id" integer NOT NULL,
  "team_id" integer NOT NULL,
  
  PRIMARY KEY ("user_id", "team_id"),
  
  FOREIGN KEY ("user_id")
    REFERENCES "users"."users" ("user_id")
	ON DELETE CASCADE,
  FOREIGN KEY ("team_id")
    REFERENCES "football"."teams" ("team_id")
	ON DELETE CASCADE
);