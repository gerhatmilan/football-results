""" This module defines the base DatabaseQuerier class and custom query functions """

from abc import ABC, abstractmethod
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH

import os
import psycopg2

from dbupdater.config.config import JSONReader

CURRENT_SEASON_COMMAND_CONFIG_FILE = 'config/other/get_current_season.json'
AVAILABLE_SEASONS_COMMAND_CONFIG_FILE = 'config/other/get_available_seasons.json'

class DatabaseQuerier():
    """ Base class for querying from a database """

    def __init__(self, db_config_file: str, cmd_config_file: str = None):
        self.read_config(db_config_file)
        self.set_command(cmd_config_file)

    def read_config(self, path: str):
        """ Loads database parameters from the given configuration file """

        try:
            reader = JSONReader()
            config_json = reader.read(path)
            self.host = config_json["host"]
            self.port = config_json["port"]
            self.database_name = config_json["database_name"]
            try:
                self.db_username = os.getenv(config_json["dbuser_env_var_name"])
                self.db_password = os.getenv(config_json["dbuser_env_var_password"])
            except Exception as e:
                raise type(e)("An error has occured while reading database credentials")
        except Exception as e:
            raise type(e)(f"An error has occured while reading config file {path}")

    def set_command(self, path: str):
        """ Reads and saves the correspoding SQL command from the configuration file """

        if path is not None:
            try:
                reader = JSONReader()
                config_json = reader.read(path)

                self.command = config_json["command"]
            except Exception as e:
                raise type(e)(f"An error has occured while reading config file {path}")

    def connect(self):
        """ Connects to the database """

        self.connection = psycopg2.connect(
            dbname=self.database_name,
            user=self.db_username,
            password=self.db_password,
            host=self.host,
            port=self.port
        )

    def disconnect(self):
        """ Disconnects from the database """
        try:
            self.connection.close()
        except:
            pass

    def query(self, query: str, parameters: tuple, result_size):
        """ Performs a query then returns the result """

        with self.connection.cursor() as cur:
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
        self.command = config_json["command"]

        try:
            self.connect()

            try:
                season = self.query(self.command, (league_id, ), result_size=1)[0]
                self.connection.commit()
                logging.log(SUCCESS_LOG_PATH, f'Current season queried for league: {league_id}\n')

                return season
            except Exception as e:
                self.connection.rollback()
                raise type(e)(f'Could not query current season for league: {league_id}, cause: {str(e)}')  
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()

    def available_seasons(self, league_id: int):
        """ Returns the available seasons for the given league """

        reader = JSONReader()
        config_json = reader.read(AVAILABLE_SEASONS_COMMAND_CONFIG_FILE)
        self.command = config_json["command"]

        try:
            self.connect()

            try:
                result = self.query(self.command, (league_id, ), result_size='all')
                self.connection.commit()
                logging.log(SUCCESS_LOG_PATH, f'Available seasons queried for league: {league_id}\n')

                return [record[0] for record in result]
            except Exception as e:
                self.connection.rollback()
                raise type(e)(f'Could not query available seasons for league: {league_id}, cause: {str(e)}')  
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()