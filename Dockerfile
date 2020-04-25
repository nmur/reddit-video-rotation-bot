# build and test stage
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.11 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore RedditVideoRotationBot.sln
RUN dotnet test -c Release RedditVideoRotationBot.sln
RUN dotnet publish -c Release -o out RedditVideoRotationBot.sln

# ffmpeg stage
FROM jrottenberg/ffmpeg:snapshot-alpine AS ffmpeg-env

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine3.11
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=ffmpeg-env /usr/local /usr/local
COPY --from=ffmpeg-env /usr/lib /usr/lib
ENTRYPOINT ["dotnet", "RedditVideoRotationBot.dll"]

