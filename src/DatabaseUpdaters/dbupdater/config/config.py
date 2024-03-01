""" This module contains the definition of the JSONReader class and config related functions """

import json

INCLUDED_LEAGUES_PATH = 'config/included_leagues_config.json'

class JSONReader():
    """ Class for reading a json file and returning the content as a json object """

    def read(self, file: str):
        """ Reads the given file and tries to return it as a JSON object """
        with open(file) as file:
            return json.load(file)
        

def get_included_leagues() -> list[int]:
    """ Returns the leagues for which further data has to be stored, for example teams, matches, etc. These leagues have to be defined in the config file """

    included_leagues= []
    try:
        reader = JSONReader()
        config_json = reader.read(INCLUDED_LEAGUES_PATH)
        for record in config_json["leagues"]:
            included_leagues.append(record["id"])
    except Exception as e:
        raise type(e)(f'An error has occured while reading config file {INCLUDED_LEAGUES_PATH}')
    
    return included_leagues

