""" This module contains the definition of the APIClient class """

from http.client import HTTPSConnection
import json
import os

from dbupdater.config.readers import JSONReader

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
        except Exception as e:
            raise type(e)(f'An error has occured while reading API config file {api_config}')
        
    def load_endpoint_config(self, endpoint_config: str):
        """ Loads the endpoint from the given config file """

        try:
            reader = JSONReader()
            config_json = reader.read(endpoint_config)
            self.endpoint = config_json['endpoint']
        except Exception as e:
            raise type(e)(f'An error has occured while reading endpoint config file {endpoint_config}')

    def request(self, *parameters):
        """ Sends an API request, either with 0 or more parameters, then returns the data if possible"""

        try:
            if len(parameters) > 0:
                url = self.endpoint.format(*parameters)
                self.connection.request(method='GET', url=url, headers=self.headers)
            else:
                self.connection.request(method='GET', url=self.endpoint, headers=self.headers)

            response = self.connection.getresponse()
            data = response.read().decode('utf-8')
            json_data = json.loads(data)
            
            if response.status == 200 and not json_data["errors"]:
                return json_data
            elif response.status == 499 or response.status == 500:
                raise Exception(json_data["message"])
            else:
                raise Exception(f'Wrong API request: {json_data["errors"]}')
        except Exception as e:
            print(type(e))
            raise type(e)('An error has occured while communicating to the API')
        finally:
            self.connection.close()    