""" Script for updating the leagues and available_seasons table in the database with data from the API """

from dbupdater.api.client import APIClient
from dbupdater.updaters import LeaguesUpdater, get_included_leagues
from dbupdater.logging import logging

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'
ENDPOINT_CONFIG_FILE = 'config/leagues/endpoint_config.json'
COMMAND_CONFIG_FILE = 'config/leagues/command_config.json'

SUCCESS_LOG_PATH = 'logs/success_logs.txt'
ERROR_LOG_PATH = 'logs/error_logs.txt'

INCLUDED_LEAGUES_PATH = 'config/included_leagues_config.json'

try:
    client = APIClient(API_CONFIG_FILE, ENDPOINT_CONFIG_FILE)
    updater = LeaguesUpdater(DATABASE_CONFIG_FILE, COMMAND_CONFIG_FILE)

    data = client.request()

    included_league_ids = get_included_leagues(INCLUDED_LEAGUES_PATH)
    updater.update(data, included_league_ids)
except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, 'Update of table football.leagues has completed successfully\n')