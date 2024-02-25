""" Script for updating the countries table in the database with data from the API """

from dbupdater.api.client import APIClient
from dbupdater.updaters import CountriesUpdater
from dbupdater.logging import logging

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'
ENDPOINT_CONFIG_FILE = 'config/countries/endpoint_config.json'
COMMAND_CONFIG_FILE = 'config/countries/command_config.json'

SUCCESS_LOG_PATH = 'logs/success_logs.txt'
ERROR_LOG_PATH = 'logs/error_logs.txt'

try:
    client = APIClient(API_CONFIG_FILE, ENDPOINT_CONFIG_FILE)

    data = client.request()

    updater = CountriesUpdater(DATABASE_CONFIG_FILE, COMMAND_CONFIG_FILE)
    updater.update(data)
except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, 'Update of table football.countries has completed successfully\n')