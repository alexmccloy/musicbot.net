FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
#RUN dotnet publish -c Release -o out
RUN dotnet build -c Debug -o out #TODO we need to do this to debug, but we should be using release mode

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Amccloy.MusicBot.Net.dll"]
