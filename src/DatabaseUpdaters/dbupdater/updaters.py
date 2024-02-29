""" 
This module contains the definition of the database updater classes

"""

from abc import ABC, abstractmethod
from psycopg2 import extras

from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH
from dbupdater.config.config import JSONReader
from dbupdater.queriers import DatabaseQuerier


class DatabaseUpdater(ABC, DatabaseQuerier):
    """ Abstract base class for classes that implement a logic of updating the database """     

    @abstractmethod
    def update(self, data):
        """ Updates the database with the given data """
        pass

class CountriesUpdater(DatabaseUpdater):
    """ Class for updating the countries table in the database """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new CountriesUpdater object, using the given configurations """

        super().__init__(db_config_file, cmd_config_file)

    def __get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []
        for record in data["response"]:
            fields = {
                "country_id": record["name"],
                "flag_link": record["flag"]
            }
            records_list.append(fields)

        return records_list

    def get_record_for_id(self, id):
        """

        Returns the columns for the given id record
        The id must be unique in the table

        """

        records_list = [record for record in self.get_records_from_db() if record['country_id'] == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in countries table for id {id}')
        else:
            return records_list[0]

    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self.__cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self.__connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New country inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self.__connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert country with data {record}, cause: {str(e)}\n')

    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = new_record['country_id']
        old_record = self.get_record_for_id(id)

        commands_and_newvalues = [(f"{column} = %s", new_record[column]) for column in new_record.keys() if new_record[column] != old_record[column]]
        
        set_commands = [item[0] for item in commands_and_newvalues]
        new_values = [item[1] for item in commands_and_newvalues]

        sql_placeholder_value = ", ".join(set_commands)

        # at least one field is different in the new record
        if len(new_values) > 0:
            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.append(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()
                logging.log(SUCCESS_LOG_PATH, f'Country updated with id {id}: {new_record}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update country with id {id}, cause: {str(e)}\n')

    def get_fields_list(self, data):
        """ Returns the list of records to be inserted in the table """

        fields_list = []
        for record in data["response"]:
            fields = (record["name"], record["flag"])
            fields_list.append(fields)

        return fields_list
    
    def update(self, data):
        """ Function for updating the countries table in the database, with the given data """

        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)   
                existing_ids = self.get_all_ids()
                
                self.total_changes = 0

                for record in records_to_insert:
                    id = record['country_id']
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)

        logging.log(SUCCESS_LOG_PATH, f'Update of table football.countries has completed successfully, changes: {self.total_changes}\n')
                        
class LeaguesUpdater(DatabaseUpdater):
    """ Class for updating the leagues and available_seasons table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new LeaguesUpdater object, using the given configurations """

        super()._read_db_config(db_config_file)
        super()._set_commands(cmd_config_file)

    def update_leagues(self, record):
        league_id = record["league"]["id"]
        country_id = record["country"]["name"]
        name = record["league"]["name"]
        type = record["league"]["type"]
        current_season = None
        for season in record["seasons"]:
            if season["current"]:
                current_season = season["year"]
                break
        logo_link = record["league"]["logo"]
        
        fields = (league_id, country_id, name, type, current_season, logo_link)

        try:
            self.cur.execute(self.command["leagues"], fields)
            self.connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New league inserted: {fields}\n')
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert league with data {fields}, cause: {str(e)}\n')

    def update_available_seasons(self, seasons_data, league_id: int):
        for record in seasons_data:
            if record["coverage"]["fixtures"]["events"] and record["coverage"]["standings"] \
                and record["coverage"]["top_scorers"]:
                    fields = (league_id, record["year"])
                    try:
                        self.cur.execute(self.command["seasons"], fields)
                        self.connection.commit()
                        logging.log(SUCCESS_LOG_PATH, f'New available_season inserted: {fields}\n')
                    except Exception as e:
                        self.connection.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert available_season with data {fields}, cause: {str(e)}\n')

    def update(self, data, included_league_ids: list[int]):
        """ Function for updating the leagues table in the database, with the given data """

        with self._connect() as conn:
            with conn.cursor() as cur:
                self.cur = cur

                for record in data["response"]:
                    if record["league"]["id"] in included_league_ids:
                        self.update_leagues(record)
                        self.update_available_seasons(record["seasons"], record["league"]["id"])

class TeamsUpdater(DatabaseUpdater):
    """ Class for updating the teams and venues table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new TeamsUpdater object, using the given configurations """

        super()._read_db_config(db_config_file)
        super()._set_commands(cmd_config_file)

    def update_venues(self, record):
        """ This function contains the logic of how the venue data has to be inserted to the venues table """

        venue_id = record["venue"]["id"]
        country_id = record["team"]["country"]
        city = record["venue"]["city"]
        name = record["venue"]["name"]
        fields = (venue_id, country_id, city, name)
        
        try:
            self.cur.execute(self.command["venues"], fields)
            self.connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New venue inserted: {fields}\n')
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert venue with data {fields}, cause: {str(e)}\n')

    def update_teams(self, record):
        """ This function contains the logic of how the teams data has to be inserted to the teams table"""

        team_id = record["team"]["id"]
        country_id = record["team"]["country"]
        venue_id = record["venue"]["id"]
        name = record["team"]["name"]
        short_name = record["team"]["code"]
        logo_link = record["team"]["logo"]
        national = record["team"]["national"]    
        fields = (team_id, country_id, venue_id, name, short_name, logo_link, national)
        
        try:
            self.cur.execute(self.command["teams"], fields)
            self.connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New team inserted: {fields}\n')
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert team with data {fields}, cause: {str(e)}\n')

    def update(self, data):
        """ Function for updating the venues and teams tables in the database, with the given data """

        with self._connect() as conn:
            with conn.cursor() as cur:
                self.cur = cur

                for record in data["response"]:
                    self.update_venues(record)
                    self.update_teams(record)

class StandingsUpdater(DatabaseUpdater):
    """ Class for updating the standings table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new StandingsUpdater object, using the given configurations """

        super().__init__(db_config_file, cmd_config_file)

    def __get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []   
        league_data = data["response"][0]["league"]
        league_id = league_data["id"]
        season = league_data["season"]

        for standings_group in league_data["standings"]:
            for record in standings_group:
                fields = {
                    "league_id": league_id,
                    "season": season,
                    "team_id": record["team"]["id"],
                    "rank": record["rank"],
                    "group": record["group"],
                    "points": record["points"],
                    "played": record["all"]["played"],
                    "wins": record["all"]["win"],
                    "draws": record["all"]["draw"],
                    "losses": record["all"]["lose"],
                    "scored": record["all"]["goals"]["for"],
                    "conceded": record["all"]["goals"]["against"] 
                }
                records_list.append(fields)

        return records_list

    def get_record_for_id(self, id):
        """

        Returns the columns for the given ID record.
        The ID must be unique in the table.
        ID must be in (league_id, season, team_id) format.
        
        """

        records_list = [record for record in self.get_records_from_db() if (record['league_id'], record['season'], record['team_id']) == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in countries table for id {id}')
        else:
            return records_list[0]

    def get_all_ids(self) -> list[tuple]:
        """ Returns all ids in the table """

        if not self._records:
            self._records = self.get_records_from_db()

        return [(record["league_id"], record["season"], record["team_id"]) for record in self._records]

    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self.__cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self.__connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New standings inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self.__connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert standings with data {record}, cause: {str(e)}\n')

    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = (new_record['league_id'], new_record['season'], new_record['team_id'])
        old_record = self.get_record_for_id(id)

        commands_and_newvalues = [(f"{column} = %s", new_record[column]) for column in new_record.keys() if new_record[column] != old_record[column]]
        
        set_commands = [item[0] for item in commands_and_newvalues]
        new_values = [item[1] for item in commands_and_newvalues]

        # because 'group' is a reserved keyword in postresql, it needs to be replaced with \"group\"
        for idx, command in enumerate(set_commands):
            if "group" in command:
                set_commands[idx] = command.replace("group", "\"group\"")

        sql_placeholder_value = ", ".join(set_commands)

        # at least one field is different in the new record
        if len(new_values) > 0:
            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.extend(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()
                logging.log(SUCCESS_LOG_PATH, f'Standings updated with id {id}: {new_record}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update standings with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the standings table in the database, with the given data """
        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)   
                existing_ids = self.get_all_ids()

                self.total_changes = 0

                for record in records_to_insert:
                    id = (record["league_id"], record["season"], record["team_id"])
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)

class TopScorersUpdater(DatabaseUpdater):
    """ Class for updating the top_scorers table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new TopScorersUpdater object, using the given configurations """

        super()._read_db_config(db_config_file)
        super()._set_commands(cmd_config_file)
       
    def get_fields_list(self, data):
        """ Returns the list of records to be inserted in the table """

        fields_list = []
        for idx, record in enumerate(data["response"]):
            player_data = record["player"]
            statistics_data = record["statistics"][0]

            league_id = statistics_data["league"]["id"]
            season = statistics_data["league"]["season"]
            player_id = player_data["id"]
            rank = idx + 1
            played = statistics_data["games"]["appearences"]
            goals = statistics_data["goals"]["total"]
            assists = statistics_data["goals"]["assists"]

            fields = (league_id, season, player_id, rank, played, goals, assists)
            fields_list.append(fields)
        return fields_list
    
    def update(self, data):
        """ Function for updating the top_scorers table in the database, with the given data """

        with self._connect() as conn:
            with conn.cursor() as cur:
                fields_list = self.get_fields_list(data)
                for fields in fields_list:
                    try:
                        cur.execute(self.command, fields)
                        conn.commit()
                        logging.log(SUCCESS_LOG_PATH, f'New top scorer inserted: {fields}\n')
                    except Exception as e:
                        conn.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert top scorer with data {fields}, cause: {str(e)}\n')         

class PlayersUpdater(DatabaseUpdater):
    """ Class for updating the players table in the database """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new PlayersUpdater object, using the given configurations """

        super().__init__(db_config_file) 

    def get_fields_list(self, data):
        """ Returns the list of records to be inserted in the table """

        fields_list = []
        team_id = data["response"][0]["team"]["id"]
        for player_record in data["response"][0]["players"]:
            player_id = player_record["id"]
            name = player_record["name"]
            age = player_record["age"]
            number = player_record["number"]
            position = player_record["position"]
            photo_link = player_record["photo"]

            fields = (player_id, team_id, name, age, number, position, photo_link)
            fields_list.append(fields)

        return fields_list
    
    def update(self, data):
        """ Function for updating the players table in the database, with the given data """
        
        with self._connect() as conn:
            with conn.cursor() as cur:
                fields_list = self.get_fields_list(data)
                for fields in fields_list:
                    try:
                        cur.execute(self._commands["insert_command"], fields)
                        conn.commit()
                        logging.log(SUCCESS_LOG_PATH, f'New player inserted: {fields}\n')
                    except Exception as e:
                        conn.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert player with data {fields}, cause: {str(e)}\n')         

class MatchesUpdater(DatabaseUpdater):
    """ Class for updating the matches table in the database """
    
    def __init__(self, db_config_file, cmd_config_file):
        """ Initializes a new MatchesUpdater object, using the given configurations """

        super()._read_db_config(db_config_file)
        super()._set_commands(cmd_config_file)
       
    def get_fields_list(self, data):
        """ Returns the list of matches and venues records to be inserted in the table """

        matches_fields_list = []
        venues_fields_list = []
        for record in data["response"]:
            fixture_data = record["fixture"]
            league_data = record["league"]
            teams_data = record["teams"]
            goals_data = record["goals"]

            match_id = fixture_data["id"]
            date = fixture_data["date"]
            venue_id = fixture_data["venue"]["id"]
            league_id = league_data["id"]
            season = league_data["season"]
            round = league_data["round"]
            home_team_id = teams_data["home"]["id"]
            away_team_id = teams_data["away"]["id"]
            status = fixture_data["status"]["short"]
            minute = fixture_data["status"]["elapsed"]
            home_team_goals = goals_data["home"]
            away_team_goals = goals_data["away"]

            venue_name = fixture_data["venue"]["name"]
            venue_city = fixture_data["venue"]["city"]
            venue_country = league_data["country"]

            matches_fields = (match_id, date, venue_id, league_id, season, round, home_team_id, away_team_id, status, minute, home_team_goals, away_team_goals)
            venue_fields = (venue_id, venue_country, venue_city, venue_name)
            
            matches_fields_list.append(matches_fields)
            venues_fields_list.append(venue_fields)

        return matches_fields_list, venues_fields_list

    def update_matches(self, fields):
        """ Inserts a record to matches table """

        try:
            self.cur.execute(self.command["matches"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New match inserted: {fields}\n')
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert match with data {fields}, cause: {str(e)}\n')         
        
    def update_venues(self, fields):
        """ Inserts a record to venues table """

        try:
            self.cur.execute(self.command["venues"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New venue inserted: {fields}\n')
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert venue with data {fields}, cause: {str(e)}\n')

    def update(self, data):
        """ Function for updating the matches table in the database, with the given data """

        with self._connect() as conn:
            with conn.cursor() as cur:
                self.cur = cur
                matches_fields_list, venues_fields_list = self.get_fields_list(data)

                for match_fields, venue_fields in zip(matches_fields_list, venues_fields_list):
                    self.update_venues(venue_fields)                            
                    self.update_matches(match_fields)
