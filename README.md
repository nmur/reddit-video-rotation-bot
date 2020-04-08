# reddit-video-rotation-bot
Reddit bot that rotates videos.

## Building
1. `dotnet publish` the project
2. `docker build -t nmur/reddit-video-rotation-bot .`
3. `docker login`
4. `docker push nmur/reddit-video-rotation-bot:latest` 