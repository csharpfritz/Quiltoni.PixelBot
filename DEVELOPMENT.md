# Developing on the Quiltoni.Pixelbot


## Environment

To effectively work on development of this bot, you will need to get a [Twitch.tv][] account setup, to test and validate any operations requiring credentials.


## Configuration

Ensure that your IDE or editor of choice respects the configuration and settings in the `.editorconfig` file.

Additionally, before creating a pull request for your changes, make sure that all unit tests pass.

## Tooling

In order to test locally in a development environment, particularly webhooks from the [Twitch][] service, it is recommended to use [NGrok][], which will create a publicly accessible URL which will tunnel back into your local system on the port(s) specified.

## Reference

* The [Twitch][] [dev site](https://dev.twitch.tv/)
* The [Twitch][] [documentation](https://dev.twitch.tv/docs/)
* Using the [Twitch][] [webhooks](https://dev.twitch.tv/docs/api/webhooks-reference)

<!-- Reference style link definitions below  -->

[Twitch]: https://twitch.tv
[Twitch.tv]: https://twitch.tv
[NGrok]: https://ngrok.io
