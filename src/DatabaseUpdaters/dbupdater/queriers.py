from dbupdater.base import DatabaseQuerier
from dbupdater.logging import logging

SUCCESS_LOG_PATH = 'logs/success_logs.txt'
ERROR_LOG_PATH = 'logs/error_logs.txt'

class CurrentSeasonGetter(DatabaseQuerier):
    """ Class which returns the current season for a given league """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new CountriesUpdater object, using the given configurations """

        self.command: str

        super().read_config(db_config_file)
        super().set_command(cmd_config_file)    

    def current_season(self, league_id: int) -> int:
        """ Returns the current season for the given league """

        try:
            self.connect()
            try:
                season = self.query(self.command, (league_id, ))[0]
                self.connection.commit()
                logging.log(SUCCESS_LOG_PATH, f'Current season queried for league: {league_id}\n')

                return season
            except Exception as e:
                self.connection.rollback()
                raise type(e)(f'Could not query current season for league: {league_id}, cause: {str(e)}')  
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()