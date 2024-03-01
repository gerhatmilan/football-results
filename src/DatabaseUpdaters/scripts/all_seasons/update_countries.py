""" Script for updating the countries table in the database with data from the API """

from dbupdater.api.client import APIClient, get_data
from dbupdater import updaters
from dbupdater.updaters import CountriesUpdater
from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH

DATABASE_CONFIG_FILE = 'config/database_config.json'
API_CONFIG_FILE = 'config/api_config.json'

# 'API' for calling the API to save the data to defined path, and also updating the database
# 'FILE' for reading the saved data from the file, and updating the database
MODE = 'FILE'

try:
    client = APIClient(API_CONFIG_FILE, updaters.COUNTRIES_CONFIG_FILE)
    updater = CountriesUpdater(DATABASE_CONFIG_FILE, updaters.COUNTRIES_CONFIG_FILE)
    
    data = get_data(client=client, mode=MODE, save=True, config=updaters.COUNTRIES_CONFIG_FILE)

    updater.update(data)

except Exception as e:
    logging.log(ERROR_LOG_PATH, str(e) + "\n")
    exit()

logging.log(SUCCESS_LOG_PATH, f'Update of table football.countries has completed successfully, changes: {updater.total_changes}\n')