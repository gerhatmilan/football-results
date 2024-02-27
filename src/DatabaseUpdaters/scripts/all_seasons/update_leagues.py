""" Script for updating the leagues and available_seasons table in the database with data from the API """

from dbupdater.api.client import APIClient, get_data
from dbupdater.updaters import LeaguesUpdater
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH
from dbupdater.config.config import get_included_leagues, INCLUDED_LEAGUES_PATH


DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'
LEAGUES_CONFIG_FILE = 'config/leagues.json'

# 'API' for calling the API to save the data to defined path, and also updating the database
# 'FILE' for reading the saved data from the file, and updating the database
MODE = 'FILE'

try:
    client = APIClient(API_CONFIG_FILE, LEAGUES_CONFIG_FILE)
    updater = LeaguesUpdater(DATABASE_CONFIG_FILE, LEAGUES_CONFIG_FILE)
    included_league_ids = get_included_leagues(INCLUDED_LEAGUES_PATH)

    data = get_data(client=client, mode=MODE, save=True, config=LEAGUES_CONFIG_FILE)

    updater.update(data, included_league_ids)
except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, 'Update of table football.leagues and available_seasons has completed successfully\n')