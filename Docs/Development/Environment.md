# Setting Up The Development Environment

## Language And Frameworks

The FritzBot is written in [C#](https://docs.microsoft.com/dotnet/csharp), using the [.Net Core](https://dotnet.microsoft.com/learn/dotnet/what-is-dotnet) framework v3.0 targetting [ASP.net core](https://asp.net). It is also written as a [Blazor](https://blazor.net) project.

## Tooling

Any development environment or editor which can work with C# and Dotnet Core will do. The recommended development environment is [Visual Studio][]. That can be either [Visual Studio Code][] (aka VSCode) and [Visual Studio][Visual Studio for PC] (any edition). [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac) does not yet support developing [Blazor][] projects. So if you are on a Mac, [Visual Studio Code][] is your best option.

If you prefer an alternate editor or <abbr title="Integrated Development Environment">IDE</abbr>, please make sure that it can make use of, and respect, the project file settings and preferences specified in an [.editorconfig][] file

Additionally, since the project is intended to be run within a container (i.e. _[Docker][]_), you may want to do your development within a containerized environment to ensure that your changes behave as expected when executed from within a container.

## Updating The Configuration Files And Environment For Development

In order for the project to be able to work with your specific information (API keys, channel name, user name), you will need to edit the `appsettings.development.json` file in the `Quiltoni.PixelBot` project directory. The relevant items are:

* `UserName`: The username which should be used for the bot
* `Channel`: The name of the channel for the bot to join

Additional, one value is set in a separate, private file on your local development maching due to its sensitivity. That property is `AccessToken`, described below. You can find out more on using the `secrets.json` file from the [App secrets in ASP.net Core][] page.

* `AccessToken`: Your [Twitch][] chat token. See [GettingStarted.md][] for information on generating one

In addition to, or in lieu of, updating the `appsettings.development.json` file, you can also set environment varibles to the appropriate values. This is particularly helpful when working with containers, rather than embedding your configurations and secrets within the container. The key names of the `.json` file map to the following environment variables:
* `UserName` is mapped to `Twitch__UserName`
* `Channel` is mapped to `Twitch__Channel`
* `AccessToken` is maped to `Twitch__AccessToken`



{{ include References.md }}
