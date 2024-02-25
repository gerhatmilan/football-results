""" Module containing functions for logging """

from datetime import datetime

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