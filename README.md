## About The Project

This is a football themed live score web application based on ASP.NET Core.

I made this project as my final exam for my Computer Science BSc studies at Eötvös Loránd University.
My main goal was to get more familiar with building a complete and functional web application from scratch, including design, research, development and deployment.


### Built With

[![DOTNET]][DOTNET-url]
[![Blazor]][Blazor-url]
[![Postgres]][Postgres-url]
[![HTML5]][HTML5-url]
[![CSS3]][CSS3-url]
[![Javascript]][Javascript-url]
[![Git]][Git-url]
[![Docker]][Docker-url]

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

#### Docker Desktop
Required to run Docker containers locally. Docker Desktop includes Docker Engine, Docker CLI, and Docker Compose, providing everything needed to build, run, and manage the application in containers on your own machine. You can download it from [Docker's official site](https://www.docker.com/get-started).

#### Football API key
The project uses a public API to get the newest football data: https://www.api-football.com/\
After signing up you will be able to see your own API key at https://dashboard.api-football.com/profile?access. You will need this during configuration.\
I used the <b>Free Pricing Tier</b> for demonstration purposes, consider that <b>this only lets you make 100 API calls each 24 hours</b>, which makes the application somewhat limited.

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/gerhatmilan/football-results
   ```
   
2. Set up <b>Configuration/sharedSettings.json</b> based on your needs. You can set:
    * The rate limit that the API currently uses for your account (using the free tier it will be 10 api calls/minute). This is important because the application needs to know how frequently it can make the requests.
    * Whether the application should save the api data and where it should save them
    * Which football leagues should the application get the data for
    * How frequently should the application update the data

3. Create a file named <b>.env</b> at the root folder, which will store the required environment variables, and fill it with the following data:

    3.1 An username for setting up the superuser for the PostgreSQL database
    ```sh
    POSTGRES_USER={your choice}
    ```
    3.2 A password for setting up the superuser for the PostgreSQL database
    ```sh
    POSTGRES_PASSWORD={your choice}
    ```
    3.3 A name for setting up the default database
    ```sh
    POSTGRES_DB={your choice}
    ```
    3.4 Connection string which the webapp will use to connect to the database
    ```sh
    ConnectionStrings__DefaultConnection=Host=database;Port=5432;Database={your choice at 3.3};Username={your choice at 3.1};Password={your choice at 3.2}
    ```
    3.5 The API key that was assigned to you when signing up at https://www.api-football.com/
    ```sh
    FootballApiConfig__ApiKey={your api key}
    ```

    <b>It is important that you can not change the key of the variables and you provide all the required parameters in the connection string!</b>
    
4. Create the docker containers with Docker Compose (from the root folder)
    ```sh
    docker compose -f docker-compose.yml up -d
    ```

## Usage

If everything was set up correctly, you can access the web applicaton from your browser at http://localhost/

At first, the website will not show any data. This is because you have to fetch the necessary data from the API manually (the reason for this is the rate limit for the free pricing tier). The docker container with the name "football-results-dbupdater" contains a console application where you can fetch the necessary data from the API. For this, you have to:
1. Enter Docker Desktop
2. Go to Containers/football-results/dbupdater
3. Access the file system of the container under <b>Exec</b> menupoint
4. Check if you are in the correct directory (app) and start the console application:

   ```sh
   dotnet FootballResults.DatabaseUpdater.dll
   ```

## License

Distributed under the MIT License. See `LICENSE.txt` for more information.



[DOTNET]: https://img.shields.io/badge/DOTNET-512BD4?style=for-the-badge&logo=dotnet&logoColor=ffffff
[DOTNET-url]: https://dotnet.microsoft.com/en-us/
[Blazor]: https://img.shields.io/badge/blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=ffffff
[Blazor-url]: https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor
[Postgres]: https://img.shields.io/badge/postgres-4169E1?style=for-the-badge&logo=postgresql&logoColor=ffffff
[Postgres-url]: https://www.postgresql.org/
[HTML5]: https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=ffffff
[HTML5-url]: https://www.w3schools.com/html/
[CSS3]: https://img.shields.io/badge/CSS3-1572B6?style=for-the-badge&logo=css3&logoColor=ffffff
[CSS3-url]: https://www.w3schools.com/css/
[Javascript]: https://img.shields.io/badge/javascript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=ffffff
[Javascript-url]: https://developer.mozilla.org/en-US/docs/Web/JavaScript
[Git]: https://img.shields.io/badge/git-F05032?style=for-the-badge&logo=git&logoColor=ffffff
[Git-url]: https://git-scm.com/
[Docker]: https://img.shields.io/badge/docker-2496ED?style=for-the-badge&logo=docker&logoColor=ffffff
[Docker-url]: https://www.docker.com/