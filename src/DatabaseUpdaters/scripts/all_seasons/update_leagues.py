""" Script for updating the leagues and available_seasons table in the database with data from the API """

from dbupdater.api.client import APIClient, get_data
from dbupdater import updaters
from dbupdater.updaters import LeaguesUpdater
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH


DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'

# 'API' for calling the API to save the data to defined path, and also updating the database
# 'FILE' for reading the saved data from the file, and updating the database
MODE = 'API'

try:
    client = APIClient(API_CONFIG_FILE, updaters.LEAGUES_CONFIG_FILE)
    updater = LeaguesUpdater(DATABASE_CONFIG_FILE, updaters.LEAGUES_CONFIG_FILE)

    data = get_data(client=client, mode=MODE, save=True, config=updaters.LEAGUES_CONFIG_FILE)

    updater.update(data)
except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, f'Update of table football.leagues and available_seasons has completed successfully, total changes: {updater.total_changes}\n')