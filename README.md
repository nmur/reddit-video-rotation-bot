# reddit-video-rotation-bot
A summonable Reddit bot that rotates videos.

## Usage
In a Reddit video post, create a reply with a user mention of the bot, eg.  
> /u/video-rotator   

Currently the username of the bot is [/u/video-rotator](https://www.reddit.com/user/video-rotator)

## Running your own instance of the bot
Currently, the Docker image is not publicly available, however building the image is straightforward.

### Building
1. Download, install, and run [Docker](https://docs.docker.com/get-docker/).
2. Clone the repo:  
    ```   
    git clone https://github.com/nmur/reddit-video-rotation-bot.git
    ```
3. Build the Docker image:
    ```   
    docker build -t reddit-video-rotation-bot .
    ```
    
### Running
1. Create a Reddit account that will respond to user mentions with the rotated video's URL.
2. Navigate to your account's [application preferences page](https://www.reddit.com/prefs/apps).
3. Create a developer app for your account:  
    1. Click the `create app` button.
    2. Select `script` type.
    3. Enter `https://not-an-aardvark.github.io/reddit-oauth-helper/` as the redirect URL (this is for refresh token generation).
    4. Fill in the other details as required, and create the app.
4. Navigate to `https://not-an-aardvark.github.io/reddit-oauth-helper/`.
5. Generate a refresh token:
    1. Under the Generate Token section, fill in your App's ID and Secret values that you just generated.
    2. Select "Permanent"
    3. Select all scopes (TODO: determine the scope(s) actually needed).
    4. Click the Generate Tokens button, and record your Refresh Token.
6. Create a [GfyCat account](https://gfycat.com/signup).
7. Register for [GfyCat API access](https://developers.gfycat.com/signup/#/apiform). The credentials will be emailed to you.
8. Open `dockervars.list`.
9. Fill in the missing fields with the values obtained in the previous steps.
10. Finally, run the Docker image:
    ```   
    docker run --env-file ./dockervars.list reddit-video-rotation-bot
    ```

## Unit tests
Unit tests are run during the Docker build process. You can alternatively run and debug the entire suite using a test runner such as the one provided by Visual Studio 2019.


