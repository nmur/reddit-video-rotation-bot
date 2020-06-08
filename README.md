[![CircleCI](https://img.shields.io/circleci/build/github/nmur/reddit-video-rotation-bot)](https://circleci.com/gh/nmur/reddit-video-rotation-bot) [![Docker Pulls](https://img.shields.io/docker/pulls/nmur/reddit-video-rotation-bot)](https://hub.docker.com/r/nmur/reddit-video-rotation-bot) 

# reddit-video-rotation-bot
A summonable Reddit bot that rotates videos.

## Usage
In a Reddit video post, create a reply with a user mention of the bot, eg.  
> /u/video-rotator   

Currently the username of the bot is [/u/video-rotator](https://www.reddit.com/user/video-rotator)

## Running your own instance of the bot
1. Create a Reddit account that the bot will use to respond to posts (or use your personal account).
2. Navigate to your account's [application preference page](https://www.reddit.com/prefs/apps) and create a new app.
3. Create a [refresh token](https://github.com/reddit-archive/reddit/wiki/OAuth2#authorization) for your account's app.
4. Create a [GfyCat account](https://gfycat.com/signup) and register it for [GfyCat API access](https://developers.gfycat.com/signup/#/apiform).
5. Open `dockervars.list` and fill in the missing fields with the values obtained in the previous steps.
6. Finally, run the latest Docker image (or build and run your own, instructions below):
    ```   
    docker run --env-file ./dockervars.list nmur/reddit-video-rotation-bot
    ```
    
More detailed instructions are available [here](https://github.com/nmur/reddit-video-rotation-bot/wiki/Detailed-steps-for-running-your-own-instance-of-the-bot).

## Building
1. Download, install, and run [Docker](https://docs.docker.com/get-docker/).
2. Clone the repo:  
    ```   
    git clone https://github.com/nmur/reddit-video-rotation-bot.git
    ```
3. Build the Docker image:
    ```   
    docker build -t reddit-video-rotation-bot .
    ```

## Unit tests
Unit tests are run during the Docker build process. You can alternatively run and debug the entire suite using a test runner such as the one provided by Visual Studio 2019.


