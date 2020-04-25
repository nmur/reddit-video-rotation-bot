FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
WORKDIR /app

# Copy everything and build
COPY . ./
RUN dotnet restore RedditVideoRotationBot.sln
RUN dotnet test -c Release RedditVideoRotationBot.sln
RUN dotnet publish -c Release -o out RedditVideoRotationBot.sln

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "RedditVideoRotationBot.dll"]

