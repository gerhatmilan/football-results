""" 
This module contains the definition of the database updater classes

"""

from dbupdater.logging import logging
from dbupdater.config.readers import JSONReader
from dbupdater.base import DatabaseQuerier, DatabaseUpdater

SUCCESS_LOG_PATH = 'logs/success_logs.txt'
ERROR_LOG_PATH = 'logs/error_logs.txt'
    
def get_included_leagues(path: str) -> list[int]:
    """ Returns the leagues for which further data has to be stored, for example teams, matches, etc. These leagues have to be defined in the parameter file """

    included_leagues= []
    try:
        reader = JSONReader()
        config_json = reader.read(path)
        for record in config_json["leagues"]:
            included_leagues.append(record["id"])
    except Exception as e:
        raise type(e)(f'An error has occured while reading config file {path}')
    
    return included_leagues

class CountriesUpdater(DatabaseUpdater):
    """ Class for updating the countries table in the database """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new CountriesUpdater object, using the given configurations """

        super().read_config(db_config_file)
        super().set_command(cmd_config_file)    

    def update(self, data):
        """ Function for updating the countries table in the database, with the given data """

        try:
            self.connect()
            
            with self.connection.cursor() as cur:
                for record in data["response"]:
                    fields = (record["name"], record["flag"])
                    try:
                        cur.execute(self.command, fields)
                        self.connection.commit()
                        logging.log(SUCCESS_LOG_PATH, f'New country inserted: {fields}\n')
                    except Exception as e:
                        self.connection.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert country with data {fields}, cause: {str(e)}')         
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()        

class LeaguesUpdater(DatabaseUpdater):
    """ Class for updating the leagues and available_seasons table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new LeaguesUpdater object, using the given configurations """

        super().read_config(db_config_file)
        super().set_command(cmd_config_file)

    def update_leagues(self, record):
        league_id = record["league"]["id"]
        country_id = record["country"]["name"]
        name = record["league"]["name"]
        type = record["league"]["type"]
        current_season = None
        for season in record["seasons"]:
            if season["current"]:
                current_season = season["year"]
                break
        logo_link = record["league"]["logo"]
        
        fields = (league_id, country_id, name, type, current_season, logo_link)

        try:
            self.cur.execute(self.commands["leagues"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New league inserted: {fields}\n')
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert league with data {fields}, cause: {str(e)}')

    def update_available_seasons(self, seasons_data, league_id: int):
        for record in seasons_data:
            if record["coverage"]["fixtures"]["events"] and record["coverage"]["standings"] \
                and record["coverage"]["top_scorers"]:
                    fields = (league_id, record["year"])
                    try:
                        self.cur.execute(self.commands["seasons"], fields)
                        logging.log(SUCCESS_LOG_PATH, f'New available_season inserted: {fields}\n')
                    except Exception as e:
                        self.connection.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert available_season with data {fields}, cause: {str(e)}')


    def update(self, data, included_league_ids: list[int]):
        """ Function for updating the leagues table in the database, with the given data """

        try:
            self.connect()
            self.cur = self.connection.cursor()

            for record in data["response"]:
                if record["league"]["id"] in included_league_ids:
                    self.update_leagues(record)
                    self.update_available_seasons(record["seasons"], record["league"]["id"])
                    self.connection.commit()  
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()

class TeamsUpdater(DatabaseUpdater):
    """ Class for updating the teams and venues table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new TeamsUpdater object, using the given configurations """

        super().read_config(db_config_file)
        super().set_command(cmd_config_file)

    def update_venues(self, record):
        """ This function contains the logic of how the venue data has to be inserted to the venues table """

        venue_id = record["venue"]["id"]
        country_id = record["team"]["country"]
        city = record["venue"]["city"]
        name = record["venue"]["name"]
        fields = (venue_id, country_id, city, name)
        
        try:
            self.cur.execute(self.commands["venues"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New venue inserted: {fields}\n')
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert venue with data {fields}, cause: {str(e)}')

    def update_teams(self, record):
        """ This function contains the logic of how the teams data has to be inserted to the teams table"""

        team_id = record["team"]["id"]
        country_id = record["team"]["country"]
        venue_id = record["venue"]["id"]
        name = record["team"]["name"]
        short_name = record["team"]["code"]
        logo_link = record["team"]["logo"]
        national = record["team"]["national"]    
        fields = (team_id, country_id, venue_id, name, short_name, logo_link, national)
        
        try:
            self.cur.execute(self.commands["teams"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New team inserted: {fields}\n')
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert team with data {fields}, cause: {str(e)}')

    def update(self, data):
        """ Function for updating the venues and teams tables in the database, with the given data """

        try:
            self.connect()
            self.cur = self.connection.cursor()

            for record in data["response"]:
                self.update_venues(record)
                self.update_teams(record)
                self.connection.commit()     
        except Exception as e:
            raise type(e)(str(e))
        finally:
            try:
                self.disconnect()
                self.cur.close()
            except:
                pass

class TopScorersUpdater(DatabaseUpdater):
    """ Class for updating the top_scorers table in the database """

    def __init__(self, db_config_file):
        """ Initializes a new TopScorersUpdater object, using the given configurations """

        super().read_config(db_config_file)
       
    def update(self, league_id, seaso, rank, player_id, played, goals, assists):
        pass

class StandingsUpdater(DatabaseUpdater):
    """ Class for updating the standings table in the database """
    
    def __init__(self, db_config_file):
        """ Initializes a new StandingsUpdater object, using the given configurations """

        super().read_config(db_config_file)
       
    def update(self, league_id, season, rank, team_id, points, played, wins, draws, losses, scored, condeded):
        pass

class PlayersUpdater(DatabaseUpdater):
    """ Class for updating the players table in the database """
    
    def __init__(self, db_config_file):
        """ Initializes a new PlayersUpdater object, using the given configurations """

        super().read_config(db_config_file)
       
    def update(self, player_id, team_id, name, age, number, position, photo_link):
        pass

class MatchesUpdater(DatabaseUpdater):
    """ Class for updating the matches table in the database """
    
    def __init__(self, db_config_file):
        """ Initializes a new MatchesUpdater object, using the given configurations """

        super().read_config(db_config_file)
       
    def update(self, match_id, date, venue_id, league_id, season, round, home_team_id, away_team_id, status, minute, home_team_goals, away_team_goals, last_update):
        pass