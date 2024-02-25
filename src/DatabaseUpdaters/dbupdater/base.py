""" This module defines the classes for communicating with a database """

from abc import ABC, abstractmethod
from typing import Union
import os
import psycopg2

from dbupdater.config.readers import JSONReader

class DatabaseQuerier():
    """ Base class for querying from a database """
    
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

        try:
            reader = JSONReader()
            config_json = reader.read(path)

            # there are multiple commands
            if len(config_json.keys()) > 1:
                self.commands = config_json # save commands with keys
            else:
                self.command = config_json['command'] # save one command only
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

    def query(self, query: str, parameters: tuple, result_size=1):
        """ Performs a query then returns the result """

        with self.connection.cursor() as cur:
            if parameters:
                cur.execute(query, parameters)
            else:
                cur.execute(query)

            if result_size == 1:
                return cur.fetchone()
            elif result_size == 'all':
                return cur.fetchall()
            else:
                return cur.fetchmany(result_size)

class DatabaseUpdater(ABC, DatabaseQuerier):
    """ Abstract base class for classes that implement a logic of updating the database """     

    @abstractmethod
    def update(self, data):
        """ Updates the database with the given data """
        pass