""" Script for updating the matches table in the database with data from the API, for today only """

from dbupdater.api.client import APIClient, get_data
from dbupdater import updaters
from dbupdater.updaters import MatchesUpdaterForToday
from dbupdater.queriers import DatabaseQuerier 
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH

from dbupdater.config.config import get_included_leagues

import time
import datetime

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'

# 'API' for calling the API to save the data to defined path, and also updating the database
# 'FILE' for reading the saved data from the file, and updating the database
MODE = 'API'

def should_update(matches_updater) -> bool:
    """ Returns true if there are any matches that should be updated """

    statuses = ['NS', '1H', 'HT', '2H', 'ET', 'BT', 'P', 'SUSP', 'INT', 'LIVE']
    matches_started = [record for record in matches_updater.get_records_from_db() if record["date"] < datetime.datetime.now(datetime.UTC).replace(tzinfo=None)]
    matches_to_update = [record for record in matches_started if record["status"] in statuses]
    return len(matches_to_update) > 0

try:
    client = APIClient(API_CONFIG_FILE, updaters.MATCHES_TODAY_CONFIG_FILE)
    updater = MatchesUpdaterForToday(DATABASE_CONFIG_FILE, updaters.MATCHES_TODAY_CONFIG_FILE)
    
    if (should_update(updater)):
        current_date_utc = datetime.datetime.now(datetime.UTC).date()

        data = get_data(client=client, mode=MODE, save=False, config=updaters.MATCHES_TODAY_CONFIG_FILE, endpoint_parameters=(str(current_date_utc),))
        updater.update(data)
        logging.log(SUCCESS_LOG_PATH, f'Update of table matches has completed successfully for the current day, total changes: {updater.total_changes}\n')
    else:
        logging.log(SUCCESS_LOG_PATH, f'No matches needed to be updated at this time.\n')       

except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()