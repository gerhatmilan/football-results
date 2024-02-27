""" This module contains the definition of the APIClient class """

from http.client import HTTPSConnection
import json
import os
import time

from dbupdater.config.config import JSONReader

class APIClient():
    """ Class for communicating with an API as a client """

    def __init__(self, api_config: str, endpoint_config: str):
        """ Initializes a new APIClient object, using the given configurations """

        self.load_api_config(api_config)
        self.load_endpoint_config(endpoint_config)
        self.connection = HTTPSConnection(self.host_url)

    def load_api_config(self, api_config: str):
        """ Loads API parameters from the given config file """
        
        try:
            reader = JSONReader()
            config_json = reader.read(api_config)
            self.host_url = config_json["api_host_url"]
            self.api_key = os.getenv(config_json["api_key_env_variable"])
            self.headers = {
                config_json["host_url_header_key"]: self.host_url,
                config_json["api_key_header_key"]: self.api_key
            }
            self.rate_limit = config_json["rate_limit"]
        except Exception as e:
            raise type(e)(f'An error has occured while reading API config file {api_config}')
        
    def load_endpoint_config(self, endpoint_config: str):
        """ Loads the endpoint from the given config file """

        try:
            reader = JSONReader()
            config_json = reader.read(endpoint_config)
            self.endpoint = config_json['endpoint']
        except Exception as e:
            raise type(e)(f'An error has occured while reading config file {endpoint_config}')

    def save_data(self, data, save_path, save_file):
        """ Saves data to the given save_path """

        os.makedirs(save_path, exist_ok=True)

        with open(save_path + save_file, mode='w') as file:
            file.write(json.dumps(data, indent=4))

    def get_rate_limit(self):
        """ Returns the API rate limit """

        return self.rate_limit

    def request(self, save:bool, save_path: str, save_file: str, endpoint_parameters: tuple[str, ...]=None):
        """ Sends an API request, either with 0 or more parameters, saves it to the given path when save=True, then returns the data if possible"""

        try:
            if len(endpoint_parameters) > 0:
                url = self.endpoint.format(*endpoint_parameters)
                self.connection.request(method='GET', url=url, headers=self.headers)
            else:
                self.connection.request(method='GET', url=self.endpoint, headers=self.headers)

            response = self.connection.getresponse()
            data = response.read().decode('utf-8')
            json_data = json.loads(data)

            if save:
                self.save_data(json_data, save_path, save_file)
            
            if response.status == 200 and not json_data["errors"]:
                return json_data
            elif response.status == 499 or response.status == 500:
                raise Exception(json_data["message"])
            else:
                raise Exception(f'Wrong API request: {json_data["errors"]}')
        except Exception as e:
            raise type(e)('An error has occured while communicating to the API')
        finally:
            self.connection.close()


def get_data(client: APIClient, mode: str, save: bool, config: str, filename_parameters: tuple=(), endpoint_parameters: tuple=()):
    """ Returns data with the API configurations, either with an answer from the API, or from the saved file """

    try:
        reader = JSONReader()
        config_json = reader.read(config)
        save_path = config_json["save_path"]
        save_file = config_json["save_file"].format(*filename_parameters)
    except Exception as e:
        raise type(e)(f'An error has occured while reading config file {config}')

    data = None
    if mode == 'API':
        data = client.request(save=save, save_path=save_path, save_file = save_file, endpoint_parameters=endpoint_parameters)
    elif mode == 'FILE':
        with open(save_path + save_file, mode='r') as file:
            data = json.load(file)
    else:
        raise Exception('Invalid mode parameter')
    
    return data