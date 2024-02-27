""" Script for updating the teams (and venues) table in the database with data from the API """

from dbupdater.api.client import APIClient, get_data
from dbupdater.updaters import TeamsUpdater
from dbupdater.queriers import DatabaseQuerier 
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH

from dbupdater.config.config import get_included_leagues, INCLUDED_LEAGUES_PATH

import time

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'
TEAMS_CONFIG_FILE = 'config/teams.json'

# 'API' for calling the API to save the data to defined path, and also updating the database
# 'FILE' for reading the saved data from the file, and updating the database
MODE = 'FILE'

try:
    client = APIClient(API_CONFIG_FILE, TEAMS_CONFIG_FILE)
    updater = TeamsUpdater(DATABASE_CONFIG_FILE, TEAMS_CONFIG_FILE)
    querier = DatabaseQuerier(DATABASE_CONFIG_FILE)
    
    included_league_ids = get_included_leagues(INCLUDED_LEAGUES_PATH)
    current_seasons = [querier.current_season(league_id) for league_id in included_league_ids]

    for league_id, season in zip(included_league_ids, current_seasons):
            try:
                data = get_data(client=client, mode=MODE, save=True, config=TEAMS_CONFIG_FILE, filename_parameters=(league_id, season), endpoint_parameters=(league_id, season))
            except FileNotFoundError:
                 # This means the teams data is not available yet for the current season. Skip this update
                 continue
            
            if MODE == 'API':
                 time.sleep(60. / client.get_rate_limit())
            updater.update(data)
except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, 'Update of table football.teams and football.venues has completed successfully for the current season\n')