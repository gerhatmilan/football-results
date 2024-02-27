""" Module containing functions and constants for logging """

from datetime import datetime

SUCCESS_LOG_PATH = 'logs/success_logs.txt'
ERROR_LOG_PATH = 'logs/error_logs.txt'

def log(path, log):
    """ Writes the given log to the given file """

    try:
        with open(path, 'a') as file:
            try:
                file.write(date() + ": " + log)
            except:
                pass
    except:
        print('Invalid log file')
        exit()

def date():
    """ Returns the current date and time """

    return datetime.now().strftime("%Y-%m-%d %H:%M:%S")