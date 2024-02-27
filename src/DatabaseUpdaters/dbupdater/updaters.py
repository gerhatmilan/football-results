""" 
This module contains the definition of the database updater classes

"""

from abc import ABC, abstractmethod

from dbupdater.logging import logging
from dbupdater.logging.logging import SUCCESS_LOG_PATH, ERROR_LOG_PATH
from dbupdater.config.config import JSONReader
from dbupdater.queriers import DatabaseQuerier

class DatabaseUpdater(ABC, DatabaseQuerier):
    """ Abstract base class for classes that implement a logic of updating the database """     

    @abstractmethod
    def update(self, data):
        """ Updates the database with the given data """
        pass

class CountriesUpdater(DatabaseUpdater):
    """ Class for updating the countries table in the database """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new CountriesUpdater object, using the given configurations """

        super().read_db_config(db_config_file)
        super().set_command(cmd_config_file)    

    def get_fields_list(self, data):
        """ Returns the list of records to be inserted in the table """

        fields_list = []
        for record in data["response"]:
            fields = (record["name"], record["flag"])
            fields_list.append(fields)

        return fields_list
    
    def update(self, data):
        """ Function for updating the countries table in the database, with the given data """

        try:
            self.connect()
            
            with self.connection.cursor() as cur:
                fields_list = self.get_fields_list(data)
                for fields in fields_list:
                    try:
                        cur.execute(self.command, fields)
                        self.connection.commit()
                        logging.log(SUCCESS_LOG_PATH, f'New country inserted: {fields}\n')
                    except Exception as e:
                        self.connection.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert country with data {fields}, cause: {str(e)}\n')         
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()        

class LeaguesUpdater(DatabaseUpdater):
    """ Class for updating the leagues and available_seasons table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new LeaguesUpdater object, using the given configurations """

        super().read_db_config(db_config_file)
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
            self.cur.execute(self.command["leagues"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New league inserted: {fields}\n')
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert league with data {fields}, cause: {str(e)}\n')

    def update_available_seasons(self, seasons_data, league_id: int):
        for record in seasons_data:
            if record["coverage"]["fixtures"]["events"] and record["coverage"]["standings"] \
                and record["coverage"]["top_scorers"]:
                    fields = (league_id, record["year"])
                    try:
                        self.cur.execute(self.command["seasons"], fields)
                        logging.log(SUCCESS_LOG_PATH, f'New available_season inserted: {fields}\n')
                    except Exception as e:
                        self.connection.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert available_season with data {fields}, cause: {str(e)}\n')


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

        super().read_db_config(db_config_file)
        super().set_command(cmd_config_file)

    def update_venues(self, record):
        """ This function contains the logic of how the venue data has to be inserted to the venues table """

        venue_id = record["venue"]["id"]
        country_id = record["team"]["country"]
        city = record["venue"]["city"]
        name = record["venue"]["name"]
        fields = (venue_id, country_id, city, name)
        
        try:
            self.cur.execute(self.command["venues"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New venue inserted: {fields}\n')
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert venue with data {fields}, cause: {str(e)}\n')

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
            self.cur.execute(self.command["teams"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New team inserted: {fields}\n')
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert team with data {fields}, cause: {str(e)}\n')

    def update(self, data):
        """ Function for updating the venues and teams tables in the database, with the given data """

        try:
            self.connect()
            self.cur = self.connection.cursor()

            for record in data["response"]:
                self.update_venues(record)
                self.update_teams(record)
        except Exception as e:
            raise type(e)(str(e))
        finally:
            try:
                self.disconnect()
                self.cur.close()
            except:
                pass

class StandingsUpdater(DatabaseUpdater):
    """ Class for updating the standings table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new StandingsUpdater object, using the given configurations """

        super().read_db_config(db_config_file)
        super().set_command(cmd_config_file)
       
    def update(self, data):
        """ Function for updating the standings table in the database, with the given data """

        try:
            self.connect()

            with self.connection.cursor() as cur:
                league_data = data["response"][0]["league"]

                league_id = league_data["id"]
                season = league_data["season"]

                for standings_group in league_data["standings"]:
                    for record in standings_group:
                        team_id = record["team"]["id"]
                        rank = record["rank"]
                        group = record["group"]
                        points = record["points"]
                        played = record["all"]["played"]
                        wins = record["all"]["win"]
                        draws = record["all"]["draw"]
                        losses = record["all"]["lose"]
                        scored = record["all"]["goals"]["for"]
                        conceded = record["all"]["goals"]["against"]    

                        fields = (league_id, season, team_id, rank, group, points, played, wins, draws, losses, scored, conceded)
                        try:
                            cur.execute(self.command, fields)
                            self.connection.commit()
                            logging.log(SUCCESS_LOG_PATH, f'New standings inserted: {fields}\n')
                        except Exception as e:
                            self.connection.rollback()
                            logging.log(ERROR_LOG_PATH, f'Could not insert standings with data {fields}, cause: {str(e)}\n')         
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()

class TopScorersUpdater(DatabaseUpdater):
    """ Class for updating the top_scorers table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new TopScorersUpdater object, using the given configurations """

        super().read_db_config(db_config_file)
        super().set_command(cmd_config_file)
       
    def get_fields_list(self, data):
        """ Returns the list of records to be inserted in the table """

        fields_list = []
        for idx, record in enumerate(data["response"]):
            player_data = record["player"]
            statistics_data = record["statistics"][0]

            league_id = statistics_data["league"]["id"]
            season = statistics_data["league"]["season"]
            player_id = player_data["id"]
            rank = idx + 1
            played = statistics_data["games"]["appearences"]
            goals = statistics_data["goals"]["total"]
            assists = statistics_data["goals"]["assists"]

            fields = (league_id, season, player_id, rank, played, goals, assists)
            fields_list.append(fields)
        return fields_list
    
    def update(self, data):
        """ Function for updating the top_scorers table in the database, with the given data """

        try:
            self.connect()

            with self.connection.cursor() as cur:
                fields_list = self.get_fields_list(data)
                for fields in fields_list:
                    try:
                        cur.execute(self.command, fields)
                        self.connection.commit()
                        logging.log(SUCCESS_LOG_PATH, f'New top scorer inserted: {fields}\n')
                    except Exception as e:
                        self.connection.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert top scorer with data {fields}, cause: {str(e)}\n')         
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()

class PlayersUpdater(DatabaseUpdater):
    """ Class for updating the players table in the database """

    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new PlayersUpdater object, using the given configurations """

        super().read_db_config(db_config_file)
        super().set_command(cmd_config_file)   
 

    def get_fields_list(self, data):
        """ Returns the list of records to be inserted in the table """

        fields_list = []
        team_id = data["response"][0]["team"]["id"]
        for player_record in data["response"][0]["players"]:
            player_id = player_record["id"]
            name = player_record["name"]
            age = player_record["age"]
            number = player_record["number"]
            position = player_record["position"]
            photo_link = player_record["photo"]

            fields = (player_id, team_id, name, age, number, position, photo_link)
            fields_list.append(fields)

        return fields_list
    
    def update(self, data):
        """ Function for updating the players table in the database, with the given data """
        try:
            self.connect()
            
            with self.connection.cursor() as cur:
                fields_list = self.get_fields_list(data)
                for fields in fields_list:
                    try:
                        cur.execute(self.command, fields)
                        self.connection.commit()
                        logging.log(SUCCESS_LOG_PATH, f'New player inserted: {fields}\n')
                    except Exception as e:
                        self.connection.rollback()
                        logging.log(ERROR_LOG_PATH, f'Could not insert player with data {fields}, cause: {str(e)}\n')         
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()

class MatchesUpdater(DatabaseUpdater):
    """ Class for updating the matches table in the database """
    
    def __init__(self, db_config_file: str, cmd_config_file: str):
        """ Initializes a new MatchesUpdater object, using the given configurations """

        super().read_db_config(db_config_file)
        super().set_command(cmd_config_file)
       
    def get_fields_list(self, data):
        """ Returns the list of records to be inserted in the table """

        matches_fields_list = []
        venues_fields_list = []
        for record in data["response"]:
            fixture_data = record["fixture"]
            league_data = record["league"]
            teams_data = record["teams"]
            goals_data = record["goals"]

            match_id = fixture_data["id"]
            date = fixture_data["date"]
            venue_id = fixture_data["venue"]["id"]
            league_id = league_data["id"]
            season = league_data["season"]
            round = league_data["round"]
            home_team_id = teams_data["home"]["id"]
            away_team_id = teams_data["away"]["id"]
            status = fixture_data["status"]["short"]
            minute = fixture_data["status"]["elapsed"]
            home_team_goals = goals_data["home"]
            away_team_goals = goals_data["away"]

            venue_name = fixture_data["venue"]["name"]
            venue_city = fixture_data["venue"]["city"]
            venue_country = league_data["country"]

            matches_fields = (match_id, date, venue_id, league_id, season, round, home_team_id, away_team_id, status, minute, home_team_goals, away_team_goals)
            venue_fields = (venue_id, venue_country, venue_city, venue_name)
            
            matches_fields_list.append(matches_fields)
            venues_fields_list.append(venue_fields)

        return matches_fields_list, venues_fields_list

    def update_matches(self, cursor, fields):
        """ Inserts a record to matches table """

        try:
            cursor.execute(self.command["matches"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New match inserted: {fields}\n')
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert match with data {fields}, cause: {str(e)}\n')         

        
    def update_venues(self, cursor, fields):
        """ Inserts a record to venues table """

        try:
            cursor.execute(self.command["venues"], fields)
            logging.log(SUCCESS_LOG_PATH, f'New venue inserted: {fields}\n')
            self.connection.commit()
        except Exception as e:
            self.connection.rollback()
            logging.log(ERROR_LOG_PATH, f'Could not insert venue with data {fields}, cause: {str(e)}\n')

    def update(self, data):
        """ Function for updating the matches table in the database, with the given data """

        try:
            self.connect()

            with self.connection.cursor() as cur:
                matches_fields_list, venues_fields_list = self.get_fields_list(data)

                for match_fields, venue_fields in zip(matches_fields_list, venues_fields_list):
                    self.update_venues(cur, venue_fields)                            
                    self.update_matches(cur, match_fields)
        except Exception as e:
            raise type(e)(str(e))
        finally:
            self.disconnect()