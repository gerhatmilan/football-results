""" Script for updating the players table in the database with data from the API, for the current season """

from dbupdater.api.client import APIClient, get_data
from dbupdater import updaters
from dbupdater.updaters import PlayersUpdater
from dbupdater.queriers import DatabaseQuerier 
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH

import time
import sys

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'

# 'API' for calling the API to save the data to defined path, and also updating the database
# 'FILE' for reading the saved data from the file, and updating the database
MODE = 'API'

if len(sys.argv) < 3 or not sys.argv[1].isdigit() or not sys.argv[2].isdigit():
    print("Lower and upper bound of team list to be updated are required as arguments\n")
    logging.log(ERROR_LOG_PATH, "Lower and upper bound of team list to be updated are required as arguments\n")

try:
    client = APIClient(API_CONFIG_FILE, updaters.PLAYERS_CONFIG_FILE)
    updater = PlayersUpdater(DATABASE_CONFIG_FILE, updaters.PLAYERS_CONFIG_FILE)
    querier = DatabaseQuerier(DATABASE_CONFIG_FILE)
    
    lower_bound = int(sys.argv[1])
    upper_bound = int(sys.argv[2])
    team_ids = querier.get_teams()[lower_bound:upper_bound]

    for team_id in team_ids:
        data = get_data(client=client, mode=MODE, save=True, config=updaters.PLAYERS_CONFIG_FILE, filename_parameters=(team_id, ), endpoint_parameters=(team_id, ))
        
        if MODE == 'API':
                time.sleep(65. / client.get_rate_limit())
        updater.update(data)

except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, f'Update of table players has completed successfully for the current season, total changes: {updater.total_changes}\n')