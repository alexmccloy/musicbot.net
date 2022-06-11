FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Copy everything to the root app directory
WORKDIR /app
COPY ./Amccloy.MusicBot.Net ./Amccloy.MusicBot.Net
COPY ./Amccloy.MusicBot.Net.Dbo ./Amccloy.MusicBot.Net.Dbo/

# Switch to the main project directory for dotnet commands
WORKDIR /app/Amccloy.MusicBot.Net

# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
#RUN dotnet publish -c Release -o out
RUN dotnet build -c Debug -o ../out #TODO we need to do this to debug, but we should be using release mode

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Amccloy.MusicBot.Net.dll"]
