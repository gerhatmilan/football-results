#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files
COPY ["FootballResults.WebApp/FootballResults.WebApp.csproj", "FootballResults.WebApp/"]
COPY ["FootballResults.DataAccess/FootballResults.DataAccess.csproj", "FootballResults.DataAccess/"]
COPY ["FootballResults.Models/FootballResults.Models.csproj", "FootballResults.Models/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]

# Restore dependencies
RUN dotnet restore "./FootballResults.WebApp/FootballResults.WebApp.csproj"

# Copy source code
COPY . .

# Build FootballResults.WebApp
RUN dotnet build "./FootballResults.WebApp/FootballResults.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish the project
RUN dotnet publish "./FootballResults.WebApp/FootballResults.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish/ /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copy the published files
COPY --from=publish /app/publish .

# Set the entry point to the WebApp
ENTRYPOINT ["dotnet", "FootballResults.WebApp.dll"]