# Developing on the Quiltoni.Pixelbot

## Environment

To effectively work on development of this bot, you will need to get a [Twitch.tv][] account setup, to test and validate any operations requiring credentials.

## Configuration

Ensure that your IDE or editor of choice respects the configuration and settings in the `.editorconfig` file.

Additionally, before creating a pull request for your changes, make sure that all unit tests, new and existing, pass.

## Tooling

In order to test locally in a development environment, particularly webhooks from the [Twitch][] service, it is recommended to use [NGrok][], which will create a publicly accessible URL which will tunnel back into your local system on the port(s) specified.

-------

## Initial Setup

### NGrok

#### What Is NGrok, And Why Do I Need It?

Due to the nature in which the [Twitch][] API operates with postbacks, it is necessary for the development system to be accessible on the public internet over HTTP and HTTPS. For a number of security reasons, having a mechine completely open and available over the internet would be a *bad* idea. Additionally, the development server will likely not be running on the standard HTTP or HTTPS ports.

This is where [NGrok][] comes in. [NGrok][] runs a thin client on the development computer which is used to create a tunnel from a public hostname and specified port(s). It then proxies (shuttles) the network activity back and forth between the local system and the [Twitch][] API service. This allows you to keep your computer safely on your private network, as well as run the development server on the port of your choosing, since [NGrok][] does port translation. For example, if your server listening for HTTPS connections on port 5000, [NGrok][] can be run so that the public HTTP endpoint points to the private HTTPS endpoint.

#### Getting The [NGrok] Client

There are several different methods available to run the [NGrok] client, depending on your personal preference and any restrictions configured on your computer.

##### Installation

The simplest and most straightforward method is to download the [NGrok][] client from the [downloads page](https://ngrok.com/download), selecting the appropriate option for your platform, and following the installation instructions

Additionally, if you make use of package managers for your platform ([Chocolatey](), [Scoop](), [Homebrew](), [MacPorts](), etc.) there is a good chance that the [NGrok][] client can be installed from there as well.

##### Platform Agnostic

Another available option that is common across all platforms is to use an [NPM]() package. To do so, you must first have [NodeJS]() installed on your development environment.

### [Twitch.tv][] Account and API Keys


-------

## References

* The [Twitch][] [dev site](https://dev.twitch.tv/)
* The [Twitch][] [documentation](https://dev.twitch.tv/docs/)
* Using the [Twitch][] [webhooks](https://dev.twitch.tv/docs/api/webhooks-reference)

<!-- Reference style link definitions below  -->

[Twitch]: https://twitch.tv
[Twitch.tv]: https://twitch.tv
[NGrok]: https://ngrok.io
