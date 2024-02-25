""" Script for updating the teams (and venues) table in the database with data from the API """

from dbupdater.api.client import APIClient
from dbupdater.updaters import TeamsUpdater, get_included_leagues
from dbupdater.queriers import CurrentSeasonGetter
from dbupdater.logging import logging

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'
ENDPOINT_CONFIG_FILE = 'config/teams/endpoint_config.json'
TEAMS_COMMAND_CONFIG_FILE = 'config/teams/command_config.json'
CURRENT_SEASON_COMMAND_CONFIG_FILE = 'config/other/get_current_season/command_config.json'

SUCCESS_LOG_PATH = 'logs/success_logs.txt'
ERROR_LOG_PATH = 'logs/error_logs.txt'

INCLUDED_LEAGUES_PATH = 'config/included_leagues_config.json'

try:
    client = APIClient(API_CONFIG_FILE, ENDPOINT_CONFIG_FILE)
    updater = TeamsUpdater(DATABASE_CONFIG_FILE, TEAMS_COMMAND_CONFIG_FILE)
    querier = CurrentSeasonGetter(DATABASE_CONFIG_FILE, CURRENT_SEASON_COMMAND_CONFIG_FILE)
    

    included_league_ids = get_included_leagues(INCLUDED_LEAGUES_PATH)
    seasons = [querier.current_season(league_id) for league_id in included_league_ids]

    for league_id, season in zip(included_league_ids, seasons):
            data = client.request(league_id, season)
            updater.update(data)
except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, 'Update of table football.teams and football.venues has completed successfully\n')