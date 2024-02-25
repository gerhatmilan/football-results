""" This module contains the definition of the JSONReader class """

import json

class JSONReader():
    """ Class for reading a json file and returning the content as a json object """

    def read(self, file: str):
        """ Reads the given file and tries to return it as a JSON object """
        with open(file) as file:
            return json.load(file)