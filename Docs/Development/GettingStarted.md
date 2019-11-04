# Developing on the FritzBot

## Code Base

[Fork][Forking a git repository] the [main repository][project repository] ([https://github.com/csharpfritz/Quiltoni.Pixelbot/]()) to your own [Github][] account. Then clone the forked repository to your development computer.

If your IDE or editor supports Visual Studio solution files (`*.sln`), you can open the `Quiltoni.PixelBot.sln` solution in the root directory of the project. If, however, your editor does **NOT** support `*.sln` files, open the repository's root directory as the source. The startup project should be `Quiltoni.PixelBot` ([/Quiltoni.PixelBot/Quiltoni.PixelBot.csproj]()).

Before running, ensure that all [NuGet][] packages have been restored and that the build environment is configured as needed (see below).

## Environment

To effectively work on development of this bot, you will need to get a [Twitch.tv][] account setup, to test and validate any operations requiring credentials.

For more details and information, consult [Environment.md](./Environment.md).

## Configuration

Ensure that your IDE or editor of choice respects the configuration and settings in the `.editorconfig` file.

Additionally, before creating a pull request for your changes, make sure that all unit tests, new and existing, pass. Additional details and information are available in the [Contributing.md][] document.

## Tooling

In order to test locally in a development environment, particularly webhooks from the [Twitch][] service, it is recommended to use [ngrok][], which will create a publicly accessible URL which will tunnel back into your local system on the port(s) specified.

-------

## Initial Setup

### [Twitch.tv][] Account and API Keys

Given that this bot is for chat room interactions on [Twitch.tv][], you will naturally need a [Twitch][] account so that you can generate API keys and test functionality.

### [ngrok][]

While not strictly necessary, it is strongly recommended to use [ngrok][] or another such tool, at least during the development process, to allow for the [Twitch][] API callbacks to reach the development server.

For this project, the default port on which the web server is listening is `5000`. Thus command needed to use [ngrok][] is `ngrok http 5000`.

For more details and information, consult [ngrok.md](./ngrok.md].

-------

## Building and Running the Code ##

For the remainder of the steps to get up and running locally with the project, see the [][] document.


For more details and information, consult [BuildingAndRunningLocally.md](./BuildingAndRunningLocally.md].

-------

## References

* The [Twitch][] [dev site](https://dev.twitch.tv/)
* The [Twitch][] [documentation](https://dev.twitch.tv/docs/)
* Using the [Twitch][] [webhooks](https://dev.twitch.tv/docs/api/webhooks-reference)

<!-- Reference style link definitions below  -->

{{ include References.md }}
