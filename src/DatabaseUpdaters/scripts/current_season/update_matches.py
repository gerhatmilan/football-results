""" Script for updating the matches table in the database with data from the API, for the current season """

from dbupdater.api.client import APIClient, get_data
from dbupdater.updaters import MatchesUpdater
from dbupdater.queriers import DatabaseQuerier 
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH

from dbupdater.config.config import get_included_leagues, INCLUDED_LEAGUES_PATH

import time

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'
MATCHES_CONFIG_FILE = 'config/matches.json'

# 'API' for calling the API to save the data to defined path, and also updating the database
# 'FILE' for reading the saved data from the file, and updating the database
MODE = 'FILE'

try:
    client = APIClient(API_CONFIG_FILE, MATCHES_CONFIG_FILE)
    updater = MatchesUpdater(DATABASE_CONFIG_FILE, MATCHES_CONFIG_FILE)
    querier = DatabaseQuerier(DATABASE_CONFIG_FILE)
    
    included_league_ids = get_included_leagues(INCLUDED_LEAGUES_PATH)
    current_seasons = [querier.current_season(league_id) for league_id in included_league_ids]

    for league_id, season in zip(included_league_ids, current_seasons):
            try:
                data = get_data(client=client, mode=MODE, save=True, config=MATCHES_CONFIG_FILE, filename_parameters=(league_id, season), endpoint_parameters=(league_id, season))
            except FileNotFoundError:
                 logging.log(ERROR_LOG_PATH, f'Matches data for season {season} is not available yet. Skipping this update')
                 continue
            
            if MODE == 'API':
                 time.sleep(65. / client.get_rate_limit())
            updater.update(data)
except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, 'Update of table matches has completed successfully for the current season\n')