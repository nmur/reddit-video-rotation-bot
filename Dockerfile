FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY /src/RedditVideoRotationBot/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY /src/RedditVideoRotationBot/ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "RedditVideoRotationBot.dll"]

