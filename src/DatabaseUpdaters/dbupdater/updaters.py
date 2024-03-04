""" 
This module contains the definition of the database updater classes

"""

from abc import ABC, abstractmethod
from datetime import datetime

from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH
from dbupdater.queriers import DatabaseQuerier
from dbupdater.config.config import get_included_leagues

COUNTRIES_CONFIG_FILE = 'config/countries.json'
LEAGUES_CONFIG_FILE = 'config/leagues.json'
SEASONS_CONFIG_FILE = 'config/seasons.json'
VENUES_CONFIG_FILE = 'config/venues.json'
MATCHES_CONFIG_FILE = 'config/matches.json'
STANDINGS_CONFIG_FILE = 'config/standings.json'
TEAMS_CONFIG_FILE = 'config/teams.json'
TOPSCORERS_CONFIG_FILE = 'config/topscorers.json'
PLAYERS_CONFIG_FILE = 'config/players.json'

class DatabaseUpdater(ABC, DatabaseQuerier):
    """ Abstract base class for classes that implement a logic of updating the database """     

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initialization method for the abstract DatabaseUpdater class """

        super().__init__(db_config_file, cmd_config_file)
        self.total_changes = 0

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

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.append(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'Country updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update country with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the countries table in the database, with the given data """

        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No countries data to update\n")
            return

        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)   
                existing_ids = self.get_all_ids()
                
                for record in records_to_insert:
                    id = record['country_id']
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)

class SeasonsUpdater(DatabaseUpdater):
    """ Class for updating the available_seasons table in the database """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new SeasonsUpdater object, using the given configurations """
        super().__init__(db_config_file, cmd_config_file)

    def __get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []

        for league_record in data["response"]:
            if league_record["league"]["id"] in get_included_leagues():
                league_id = league_record["league"]["id"]
                for season_record in league_record["seasons"]:
                    if season_record["coverage"]["fixtures"]["events"] and season_record["coverage"]["standings"] \
                        and season_record["coverage"]["top_scorers"]:
                            
                            fields = {
                                "league_id": league_id,
                                "season": season_record["year"]
                            }
                    
                            records_list.append(fields)
        
        return records_list

    def get_record_for_id(self, id):
        """

        Returns the columns for the given id record.
        The id must be unique in the table.
        The id must be in (league_id, season) format.

        """

        records_list = [record for record in self.get_records_from_db() if (record["league_id"], record["season"]) == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in available_seasons table for id {id}')
        else:
            return records_list[0]

    def get_all_ids(self) -> list[tuple]:
        """ Returns all ids in the table """

        if self._records is None:
            self._records = self.get_records_from_db()

        return [(record["league_id"], record["season"]) for record in self._records]

    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self.__cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self.__connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New available_seasons record inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self.__connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert available_seasons record with data {record}, cause: {str(e)}\n')

    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = (new_record['league_id'], new_record['season'])
        old_record = self.get_record_for_id(id)

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.extend(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'Available season record updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update available season record with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the available_seasons table in the database, with the given data """

        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No seasons data to update\n")
            return

        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)   
                existing_ids = self.get_all_ids()

                for record in records_to_insert:
                    id = (record["league_id"], record["season"])
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)   

class LeaguesUpdater(DatabaseUpdater):
    """ Class for updating the leagues and available_seasons table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new LeaguesUpdater object, using the given configurations """

        super().__init__(db_config_file, cmd_config_file)

    def __get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []

        for league_record in data["response"]:
            if league_record["league"]["id"] in get_included_leagues():
                current_season = None
                for season_record in league_record["seasons"]:
                    if season_record["current"]:
                        current_season = season_record["year"]
                        break
                
                fields = {
                    "league_id": league_record["league"]["id"],
                    "country_id": league_record["country"]["name"],
                    "name": league_record["league"]["name"],
                    "type": league_record["league"]["type"],
                    "current_season": current_season,
                    "logo_link": league_record["league"]["logo"]
                }
                
                records_list.append(fields)

        return records_list
    
    def get_record_for_id(self, id):
        """
        Returns the columns for the given id record.
        The id must be unique in the table.

        """

        records_list = [record for record in self.get_records_from_db() if record["league_id"] == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in leagues table for id {id}')
        else:
            return records_list[0]
    
    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self.__cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self.__connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New leagues record inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self.__connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert leagues record with data {record}, cause: {str(e)}\n')
    
    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = new_record["league_id"]
        old_record = self.get_record_for_id(id)

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.append(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'League record updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update league record with id {id}, cause: {str(e)}\n')

    def update(self, data):
        """ Function for updating the leagues (and available_seasons) table in the database, with the given data """

        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No leagues data to update\n")
            return

        # Update leagues table first
        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)   
                existing_ids = self.get_all_ids()

                for record in records_to_insert:
                    id = record["league_id"]
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)

        # Then update available_seasons table too
        seasons_updater = SeasonsUpdater(self._db_config_file, SEASONS_CONFIG_FILE)
        seasons_updater.update(data)
        
        self.total_changes += seasons_updater.total_changes

class VenuesUpdater(DatabaseUpdater):
    """ Class for updating the venues table in the database """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new VenuesUpdater object, using the given configurations """
        super().__init__(db_config_file, cmd_config_file)

    def _get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []

        for record in data["response"]:
            fields = {
                "venue_id": record["venue"]["id"],
                "country_id": record["team"]["country"],
                "city": record["venue"]["city"],
                "name": record["venue"]["name"]
            }

            records_list.append(fields)

        return records_list

    def get_record_for_id(self, id):
        """

        Returns the columns for the given id record.
        The id must be unique in the table.

        """

        records_list = [record for record in self.get_records_from_db() if record["venue_id"] == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in venues table for id {id}')
        else:
            return records_list[0]

    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self._cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self._connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New venue record inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self._connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert venue record with data {record}, cause: {str(e)}\n')

    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = new_record['venue_id']
        old_record = self.get_record_for_id(id)

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.append(id)

            try:
                self._cursor.execute(command, new_values)
                self._connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'Venue record updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self._connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update venue record with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the venues table in the database, with the given data """

        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No venues data to update\n")
            return
        
        with self._connect() as conn:
            with conn.cursor() as cur:
                self._connection = conn
                self._cursor = cur

                records_to_insert = self._get_records_to_insert(data)   
                existing_ids = self.get_all_ids()

                for record in records_to_insert:
                    id = record['venue_id']
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)

class VenuesUpdaterForMatches(VenuesUpdater):
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new VenuesUpdaterForMatches object, using the given configurations """
        super().__init__(db_config_file, cmd_config_file)
    
    def _get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []

        for record in data["response"]:
            fixture_data = record["fixture"]
            league_data = record["league"]
            fields = {
                "venue_id": fixture_data["venue"]["id"],
                "country_id": league_data["country"],
                "city": fixture_data["venue"]["city"],
                "name": fixture_data["venue"]["name"]
            }

            records_list.append(fields)

        return records_list

    def update(self, data):
        """ Function for updating the venues table in the database, with the given data """

        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No matches data to update\n")
            return

        with self._connect() as conn:
            with conn.cursor() as cur:
                self._connection = conn
                self._cursor = cur

                records_to_insert = self._get_records_to_insert(data)   
                existing_ids = self.get_all_ids()

                for record in records_to_insert:
                    id = record['venue_id']
                    if id not in existing_ids:
                        self.insert_record(record)
                    # no need to update venue data in this case

class TeamsUpdater(DatabaseUpdater):
    """ Class for updating the teams and venues table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new VenuesUpdater object, using the given configurations """
        super().__init__(db_config_file, cmd_config_file)

    def __get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []

        for record in data["response"]:
            fields = {
                "team_id": record["team"]["id"],
                "country_id": record["team"]["country"],
                "venue_id": record["venue"]["id"],
                "name": record["team"]["name"],
                "short_name": record["team"]["code"],
                "logo_link": record["team"]["logo"],
                "national": record["team"]["national"]    
            }

            records_list.append(fields)

        return records_list

    def get_record_for_id(self, id):
        """

        Returns the columns for the given id record.
        The id must be unique in the table.

        """

        records_list = [record for record in self.get_records_from_db() if record["team_id"] == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in teams table for id {id}')
        else:
            return records_list[0]

    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self.__cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self.__connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New team record inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self.__connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert team record with data {record}, cause: {str(e)}\n')

    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = new_record['team_id']
        old_record = self.get_record_for_id(id)

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.append(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'Team record updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update team record with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the teams (and venues) table in the database, with the given data """

        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No teams data to update\n")
            return

        # Update venue table first
        venues_updater = VenuesUpdater(self._db_config_file, VENUES_CONFIG_FILE)
        venues_updater.update(data)
        
        # Then update teams table too
        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)   
                existing_ids = self.get_all_ids()

                for record in records_to_insert:
                    id = record['team_id']
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)
        

        self.total_changes += venues_updater.total_changes

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
        The ID must be in (league_id, season, team_id) format.
        
        """

        records_list = [record for record in self.get_records_from_db() if (record['league_id'], record['season'], record['team_id']) == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in standings table for id {id}')
        else:
            return records_list[0]

    def get_all_ids(self) -> list[tuple]:
        """ Returns all ids in the table """

        if self._records is None:
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

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            # because 'group' is a reserved keyword in postresql, it needs to be replaced with \"group\"
            for idx, command in enumerate(set_commands):
                if "group" in command:
                    set_commands[idx] = command.replace("group", "\"group\"")

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.extend(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'Standings updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update standings with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the standings table in the database, with the given data """
        
        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No standings data to update\n")
            return
        
        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)

                # to fetch only the relevant matches from the database
                league_id = records_to_insert[0]['league_id']
                season = records_to_insert[0]['season']
                self.get_records_from_db(command_parameters=(league_id, season))
                ######

                existing_ids = self.get_all_ids()

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

        super().__init__(db_config_file, cmd_config_file)

    def __get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []   

        for idx, record in enumerate(data["response"]):
            player_data = record["player"]
            statistics_data = record["statistics"][0]

            fields = {
                'league_id': statistics_data["league"]["id"],
                'season': statistics_data["league"]["season"],
                'player_name': player_data["name"],
                'photo_link': player_data["photo"],
                'team_id': statistics_data["team"]["id"],
                'rank': idx + 1,
                'played': statistics_data["games"]["appearences"],
                'goals': statistics_data["goals"]["total"],
                'assists': statistics_data["goals"]["assists"]
            }

            records_list.append(fields)
        return records_list      

    def get_record_for_id(self, id):
        """

        Returns the columns for the given ID record.
        The ID must be unique in the table.
        The ID must be in (league_id, season, rank) format.
        
        """

        records_list = [record for record in self.get_records_from_db() if (record['league_id'], record['season'], record['rank']) == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in top_scorers table for id {id}')
        else:
            return records_list[0]

    def get_all_ids(self) -> list[tuple]:
        """ Returns all ids in the table """

        if self._records is None:
            self._records = self.get_records_from_db()

        return [(record["league_id"], record["season"], record["rank"]) for record in self._records]

    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self.__cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self.__connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New top scorer inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self.__connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert top scorer with data {record}, cause: {str(e)}\n')

    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = (new_record['league_id'], new_record['season'], new_record['rank'])
        old_record = self.get_record_for_id(id)

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]       

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.extend(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'Top scorer record updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update top scorer data with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the top scorers table in the database, with the given data """
        
        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No top scorer data to update\n")
            return
        
        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)

                # to fetch only the relevant matches from the database
                league_id = records_to_insert[0]['league_id']
                season = records_to_insert[0]['season']
                self.get_records_from_db(command_parameters=(league_id, season))
                ######

                existing_ids = self.get_all_ids()

                for record in records_to_insert:
                    id = (record["league_id"], record['season'], record["rank"])
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)

class PlayersUpdater(DatabaseUpdater):
    """ Class for updating the players table in the database """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new PlayersUpdater object, using the given configurations """

        super().__init__(db_config_file, cmd_config_file)

    def __get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []   

        team_id = data["response"][0]["team"]["id"]
        for player_record in data["response"][0]["players"]:
            
            fields = {
                "team_id": team_id,
                "player_id": player_record["id"],
                "name": player_record["name"],
                "age": player_record["age"],
                "number": player_record["number"],
                "position": player_record["position"],
                "photo_link": player_record["photo"]
            }

            records_list.append(fields)

        return records_list

    def get_record_for_id(self, id):
        """

        Returns the columns for the given ID record.
        The ID must be unique in the table.
        The ID must be in (player_id, team_id) format.
        
        """

        records_list = [record for record in self.get_records_from_db() if (record['player_id'], record['team_id']) == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in players table for id {id}')
        else:
            return records_list[0]

    def get_all_ids(self) -> list[tuple]:
        """ Returns all ids in the table """

        if self._records is None:
            self._records = self.get_records_from_db()

        return [(record["player_id"], record["team_id"]) for record in self._records]

    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self.__cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self.__connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New player inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self.__connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert player with data {record}, cause: {str(e)}\n')

    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = (new_record['player_id'], new_record['team_id'])
        old_record = self.get_record_for_id(id)

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]       

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.extend(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'Player updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update player with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the players table in the database, with the given data """
        
        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No players to update\n")
            return
        
        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)   
                existing_ids = self.get_all_ids()

                for record in records_to_insert:
                    id = (record["player_id"], record["team_id"])
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)    

class MatchesUpdater(DatabaseUpdater):
    """ Class for updating the matches table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new MatchesUpdater object, using the given configurations """
        super().__init__(db_config_file, cmd_config_file)

    def __get_records_to_insert(self, data):
        """ Returns the list of records to be inserted in the table """

        records_list = []

        for record in data["response"]:
            fixture_data = record["fixture"]
            league_data = record["league"]
            teams_data = record["teams"]
            goals_data = record["goals"]

            fields = {
                "match_id": fixture_data["id"],
                "date": fixture_data["date"],
                "venue_id": fixture_data["venue"]["id"],
                "league_id": league_data["id"],
                "season": league_data["season"],
                "round": league_data["round"],
                "home_team_id": teams_data["home"]["id"],
                "away_team_id": teams_data["away"]["id"],
                "status": fixture_data["status"]["short"],
                "minute": fixture_data["status"]["elapsed"],
                "home_team_goals": goals_data["home"],
                "away_team_goals": goals_data["away"] 
            }

            records_list.append(fields)

        return records_list

    def get_record_for_id(self, id):
        """
        Returns the columns for the given id record.
        The id must be unique in the table.

        """

        records_list = [record for record in self.get_records_from_db() if record["match_id"] == id]
        if len(records_list) > 1:
            logging.log(ERROR_LOG_PATH, f'Error: there are more than one records in matches table for id {id}')
        else:
            return records_list[0]

    def insert_record(self, record):
        """ Inserts a record to the database """

        try:
            self.__cursor.execute(self._commands["insert_command"], tuple(record.values()))
            self.__connection.commit()
            logging.log(SUCCESS_LOG_PATH, f'New match record inserted: {record}\n')

            self.total_changes += 1
        except Exception as e:
            self.__connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert match record with data {record}, cause: {str(e)}\n')

    def update_record(self, new_record):
        """ Tries to update the already existing record in the database """

        id = new_record['match_id']
        old_record = self.get_record_for_id(id)

        # need to convert from 2000-01-01T00:00:00+00:00 format to 2000-01-01 00:00:00 to be able to compare    
        old_date_datetime = datetime.fromisoformat(new_record['date'])
        new_record['date'] = str(datetime.strftime(old_date_datetime, '%Y-%m-%d %H:%M:%S'))
        old_record['date'] = str(old_record['date'])

        diff_list = [(f"{column} = %s", new_record[column], old_record[column], column) for column in new_record.keys() if new_record[column] != old_record[column]]

        # at least one field is different in the new record
        if len(diff_list) > 0:
            set_commands, new_values, old_values, columns = zip(*diff_list)

            set_commands = list(set_commands)
            new_values = list(new_values)
            old_values = list(old_values)
            columns = list(columns)

            sql_placeholder_value = ", ".join(set_commands)

            command = self._commands["update_command"].format(sql_placeholder_value)
            new_values.append(id)

            try:
                self.__cursor.execute(command, new_values)
                self.__connection.commit()

                logging.log(SUCCESS_LOG_PATH, f'Match record updated with id {id}:\n')
                for column in columns:
                    logging.log(SUCCESS_LOG_PATH, f'\t{column}: {old_record[column]} --> {new_record[column]}\n')

                self.total_changes += 1
            except Exception as e:
                self.__connection.rollback()
                logging.log(ERROR_LOG_PATH, f'Could not update match record with id {id}, cause: {str(e)}\n')
    
    def update(self, data):
        """ Function for updating the matches (and venues) table in the database, with the given data """

        if data["response"] == []:
            logging.log(SUCCESS_LOG_PATH, "No matches data to update\n")
            return

        # Update venue table first
        venues_updater = VenuesUpdaterForMatches(self._db_config_file, VENUES_CONFIG_FILE)
        venues_updater.update(data)

        # Then update matches table too
        with self._connect() as conn:
            with conn.cursor() as cur:
                self.__connection = conn
                self.__cursor = cur

                records_to_insert = self.__get_records_to_insert(data)

                # to fetch only the relevant matches from the database
                league_id = records_to_insert[0]['league_id']
                season = records_to_insert[0]['season']
                self.get_records_from_db(command_parameters=(league_id, season))
                ######

                existing_ids = self.get_all_ids()

                for record in records_to_insert:
                    id = record['match_id']
                    if id not in existing_ids:
                        self.insert_record(record)
                    else:
                        self.update_record(record)
        

        self.total_changes += venues_updater.total_changes
