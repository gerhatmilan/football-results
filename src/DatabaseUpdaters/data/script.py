from dbupdater.api.client import APIClient, get_data
import json
import os
import time

API_CONFIG_FILE = 'config/api_config.json'
ENDPOINT_CONFIG_FILE = 'config/teams/endpoint_config.json'

LEAGUE_ID = 140
SEASON_FIRST = 2010
SEASON_LAST = 2022

client = APIClient(API_CONFIG_FILE, ENDPOINT_CONFIG_FILE)

#for season in [2016, 2020]:
for season in range(SEASON_FIRST, SEASON_LAST + 1):
    PATH = f'data/teams/{LEAGUE_ID}/'
    os.makedirs(PATH, exist_ok=True)

    try:
        get_data(client, mode='API', save=True, save_path=PATH, save_file=f"{season}.json", endpoint_parameters=(LEAGUE_ID, season))
        print('Success. Waiting...')
        time.sleep(60. / client.get_rate_limit())
        print('Continuing')
    except Exception as e:
        print(str(e))