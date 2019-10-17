# What Is ngrok?

## What We Use It For

Due to the nature in which the [Twitch][] API operates with postbacks, it is necessary for the development system to be accessible on the public internet over HTTP and HTTPS. For a number of security reasons, having a mechine completely open and available over the internet would be a *bad* idea. Additionally, the development server will likely not be running on the standard HTTP or HTTPS ports.

This is where [ngrok][] comes in. [ngrok][] runs a thin client on the development computer which is used to create a tunnel from a public hostname and specified port(s). It then proxies (shuttles) the network activity back and forth between the local system and the [Twitch][] API service. This allows you to keep your computer safely on your private network, as well as run the development server on the port of your choosing, since [ngrok][] does port translation. For example, if your server listening for HTTPS connections on port 5000, [ngrok][] can be run so that the public HTTP endpoint points to the private HTTPS endpoint.

## Installation Methods

There are several different methods available to run the [ngrok] client, depending on your personal preference and any restrictions configured on your computer.

### Installation

The simplest and most straightforward method is to download the [ngrok][] client from the [downloads page](https://ngrok.com/download), selecting the appropriate option for your platform, and following the installation instructions

Additionally, if you make use of package managers for your platform ([Chocolatey](), [Scoop](), [Homebrew](), [MacPorts](), etc.) there is a good chance that the [ngrok][] client can be installed from there as well.

### Platform Agnostic

Another available option that is common across all platforms is to use an [NPM]() package. To do so, you must first have [NodeJS]() installed on your development environment.

## Creating An Account (*OPTIONAL*)

While not strictly necessary to using the [ngrok][] client, there are some benefits to creating an account. Since the benefits of an account are not needed for the development of this project, the specifics will not be listed, but can be found on the [ngrok documentation site](https://ngrok.com/docs) or on the [plans page](https://ngrok.com/billing/plans) (account needed to access this page).

<!-- Link reference section -->

<!-- #include virtual="./References.md" -->
