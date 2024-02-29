""" This module defines the base DatabaseQuerier class and custom query functions """

from abc import ABC, abstractmethod
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH

import os
import psycopg2
from psycopg2 import extras

from dbupdater.config.config import JSONReader

CURRENT_SEASON_COMMAND_CONFIG_FILE = 'config/other/get_current_season.json'
AVAILABLE_SEASONS_COMMAND_CONFIG_FILE = 'config/other/get_available_seasons.json'
GET_TEAMS_COMMAND_CONFIG_FILE = 'config/other/get_teams.json'

class DatabaseQuerier():
    """ Base class for querying from a database """

    def __init__(self, db_config_file: str, cmd_config_file: str = None):
        """ Initializes a DatabaseQuerier object """

        self._read_db_config(db_config_file)
        self._set_commands(cmd_config_file)
        self._records = []

    def _read_db_config(self, path: str):
        """ Loads database parameters from the given configuration file """

        try:
            reader = JSONReader()
            config_json = reader.read(path)
            self.__host = config_json["host"]
            self.__port = config_json["port"]
            self.__database_name = config_json["database_name"]
            try:
                self.__db_username = os.getenv(config_json["dbuser_env_var_name"])
                self.__db_password = os.getenv(config_json["dbuser_env_var_password"])
            except Exception as e:
                raise type(e)("An error has occured while reading database credentials")
        except Exception as e:
            raise type(e)(f"An error has occured while reading config file {path}")

    def _set_commands(self, path: str):
        """
            Reads and saves the correspoding SQL commands from the configuration file.
        """

        if path is not None:
            try:
                reader = JSONReader()
                config_json = reader.read(path)


                self._commands = config_json["commands"]

            except Exception as e:
                raise type(e)(f"An error has occured while reading config file {path}")

    def _connect(self):
        """ Returns the database connection (and stores it) """

        return psycopg2.connect(
            dbname=self.__database_name,
            user=self.__db_username,
            password=self.__db_password,
            host=self.__host,
            port=self.__port
        )

    def get_records_from_db(self):
        """ Fetches the corresponding table from the database, and returns its records in an indexable format """

        if not self._records:
            self._records = self.query(query=self._commands["select_command"], parameters=(), result_size='all', cursor_factory=extras.DictCursor)

        return self._records

    def get_values_for_column(self, column):
        """ Returns all values in the table for the given column """

        if not self._records:
            self._records = self.get_records_from_db()

        return [record[column] for record in self._records]

    def get_all_ids(self) -> list[int]:
        """ Returns all ids in the table, if there is only one primary key """

        if not self._records:
            self._records = self.get_records_from_db()

        return [record[0] for record in self._records]

    def query(self, query: str, parameters: tuple, result_size, cursor_factory=None):
        """ Performs a query then returns the result """
        with self._connect() as conn:
            with conn.cursor(cursor_factory=cursor_factory) as cur:
                if parameters:
                    cur.execute(query, parameters)
                else:
                    cur.execute(query)

                result = None

                if result_size == 1:
                    result = cur.fetchone()
                elif result_size == 'all':
                    result = cur.fetchall()
                else:
                    result = cur.fetchmany(result_size)

                return result
        
    def current_season(self, league_id: int) -> int:
        """ Returns the current season for the given league """

        reader = JSONReader()
        config_json = reader.read(CURRENT_SEASON_COMMAND_CONFIG_FILE)
        command = config_json["commands"]["select_command"]

        try:
            season = self.query(command, (league_id, ), result_size=1)[0]
            logging.log(SUCCESS_LOG_PATH, f'Current season queried for league: {league_id}\n')

            return season
        except Exception as e:
            raise type(e)(f'Could not query current season for league: {league_id}, cause: {str(e)}')  

    def available_seasons(self, league_id: int) -> list[int]:
        """ Returns the available seasons for the given league """

        reader = JSONReader()
        config_json = reader.read(AVAILABLE_SEASONS_COMMAND_CONFIG_FILE)
        command = config_json["commands"]["select_command"]

        try:
            result = self.query(command, (league_id, ), result_size='all')
            logging.log(SUCCESS_LOG_PATH, f'Available seasons queried for league: {league_id}\n')

            return [record[0] for record in result]
        except Exception as e:
            raise type(e)(f'Could not query available seasons for league: {league_id}, cause: {str(e)}')  

    def get_teams(self) -> list[int]:
        """ Returns relevant teams for the current season from the database """

        reader = JSONReader()
        config_json = reader.read(GET_TEAMS_COMMAND_CONFIG_FILE)
        command = config_json["commands"]["select_command"]

        try:
            result = self.query(command, parameters=(), result_size='all')
            logging.log(SUCCESS_LOG_PATH, f'Team_ids queried\n')

            return [record[0] for record in result]
        except Exception as e:
            raise type(e)(f'Could not query team_ids, cause: {str(e)}')  
