""" Script for updating the top_scorers table in the database with data from the API, through all available seasons """

from dbupdater.api.client import APIClient, get_data
from dbupdater.updaters import PlayersUpdater
from dbupdater.queriers import DatabaseQuerier 
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH

import time

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'
PLAYERS_CONFIG_FILE = 'config/players.json'

# 'API' for calling the API to save the data to defined path, and also updating the database
# 'FILE' for reading the saved data from the file, and updating the database
MODE = 'FILE'

try:
    client = APIClient(API_CONFIG_FILE, PLAYERS_CONFIG_FILE)
    updater = PlayersUpdater(DATABASE_CONFIG_FILE, PLAYERS_CONFIG_FILE)
    querier = DatabaseQuerier(DATABASE_CONFIG_FILE)
    
    team_ids = None # TODO 

    for team_id in team_ids:
            try:
                data = get_data(client=client, mode=MODE, save=True, config=PLAYERS_CONFIG_FILE, filename_parameters=(team_id), endpoint_parameters=(team_id))
            except FileNotFoundError:
                 # This means the players data is not available yet for the current season. Skip this update
                 continue
            
            if MODE == 'API':
                 time.sleep(60. / client.get_rate_limit())
            updater.update(data)
except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, 'Update of table players has completed successfully for the current season\n')