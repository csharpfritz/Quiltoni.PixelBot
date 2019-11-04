# Contributing To The FritzBot Project

## Repository

The main repository is hosted on [Github][], at [
    github.com/csharpfritz/Quiltoni.Pixelbot](https://github.com/csharpfritz/Quiltoni.Pixelbot)

## Before Making Code Changes

Before you make any code changes, check for either and existing [issue][project issues] with respect to the code change.

If there is no open issue, check the [closed issues list][closed project issues] to make sure that the change doesn't revert a previous change, or hasn't already been discussed and closed as a "won't change". If none of those situations apply, [create a new issue][new project issue] so that other contributors know which portion of the system is impacted, and any necessary discussion around the change can take place.

## Standards

The code standards are defined and laid out in the [.editorconfig][] file, and should be followed. To most easily do so, use an IDE with support for `.editorconfig` files, either natively or through plugins. The major elements to follow are:

* ***DO*** Use tabs (`\t` / `0x08`) instead of spaces (`\s` /  `0x20`) for indentation
* ***DO*** use Windows style line endings (CrLF/[`\r\n`])
* ***DO NOT*** include a newline at the end of files
* ***DO*** use the following general code styles
    * Use language keywords rather than BCL types
        * `bool doThis = true;` over `Boolean doThat = false;`
        * `string name = "User";` over `String name = "Nope";`
    * Exlicitly state the visibility of functions, properties, etc.
    * Namespaces, classes, interfaces, methods and properties are named using [PascalCase][]
    * Private class and method members are named using [camelCase][]
* ***DO*** use modern C# language features, such as:
    * Use `is null` checks
        * `myVariable is null` over `myVariable == null`
    * Use inline out variable declarations when available
        * `var isParsed = int.TryParse(number, out var parsedValue);`
    * Use expression bodied members for properties and simple methods
        * `public bool IsPositive(int value) => return value => 0;`
* ***DO*** write unit tests for new code
    * Use the current unit testing framework and tools
        * [XUnit](https://www.nuget.org/packages/xunit/)
        * [Moq](https://www.nuget.org/packages/moq/)
        * [Fluent Assertions](https://www.nuget.org/packages/FluentAssertions/)
    * Make sure all tests pass before submitting a [pull request][]
* ***DO*** make your commit messages helpful, yet concise.
    * All of the commits which are part of your pull request will most likely be merged into the target branch using the `--squash` _git_ option. Meaning that all of the commit messages which are part of the pull request will also be merged together into a single message. Keeping messages concise will help the maintainers who will be merging in the code avoid the need to put on their proverbial "editor's" hat to format or clean up the merged commit message
* ***DO** make use of the [co-authored commit message][] feature of [Github] if multiple people helped or participated in the development. Such situations may be:
    * Pair programming/mentoring
    * Live-streaming the development with help, suggestions or contributions from the community
    * Students, attendees or other people who were otherwise involved with the development
    * ***DO*** respect people's privacy, and make sure you have each co-author's permission to add them to the credits.

## Pull Requests

When you are satisfied with your code changes, you can submit them to be included in the project repository using a [pull request](https://github.com/csharpfritz/Quiltoni.Pixelbot/pulls). When submitting a [pull request][], make sure that there is an existing issue for which the code change applies, and include a reference to the issue in the description. You can do this by typing `#<number>` in the description, where `<number>` is the issue's number in the [issues list][project issues].

Unless there are multiple issues all related to a single code change, avoid making multiple changes which address multiple issues in a single pull request. Rather, make one pull request with relevant code change per issue.

If there is any feedback given on the pull request, which would require additional code changes, please be as responsibe as possible, either in submitting the changes, or communicating back to discuss the changes. This can also be a response indicating that you will not have the oppotunity to make the requested changes, but that another contributor can provide a helping hand by picking up the issue/pull-request and making the appropriate changes.

## Getting and Staying in Touch ##

You are welcome to ask any questions, discuss the project or participate in its development in a number of ways. This includes during [Jeff's (_aka_ CSharpFritz)][CSharpFritz on Twitch] live streams on [Twitch][], as well as joining Jeff's [Discord server][].

{{ include References.md }}
