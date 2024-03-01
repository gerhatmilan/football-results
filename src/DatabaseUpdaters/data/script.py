from dbupdater.api.client import APIClient, get_data
import time
from dbupdater.queriers import DatabaseQuerier
from dbupdater.updaters import PlayersUpdater

API_CONFIG_FILE = 'config/api_config.json'
ENDPOINT_CONFIG_FILE = 'config/players.json'
DATABASE_CONFIG_FILE = 'config/database_config.json'
CMD_CONFIG_FILE = 'config/players.json'

client = APIClient(API_CONFIG_FILE, ENDPOINT_CONFIG_FILE)
querier = DatabaseQuerier(DATABASE_CONFIG_FILE, CMD_CONFIG_FILE)
updater = PlayersUpdater(DATABASE_CONFIG_FILE, CMD_CONFIG_FILE)

# 195 teams left
START = 272
END = 1000

team_ids = querier.get_teams()

print(team_ids[START:END])

'''
for team_id in team_ids[START:END]:
    try:
        data = get_data(client=client, mode='API', save=True, config=CMD_CONFIG_FILE, filename_parameters=(team_id, ), endpoint_parameters=(team_id, ))
        updater.update(data)
        time.sleep(6.3)
    except:
        print(f'Error occured, skipping team with id {team_id}')
'''