# Quiltoni.PixelBot
A bot to manage pixels for Quiltoni

[![Build status](https://dev.azure.com/FritzAndFriends/PixelBot/_apis/build/status/PixelBot-ASP.NET%20Core-CI)](https://dev.azure.com/FritzAndFriends/PixelBot/_build/latest?definitionId=10)

## Attribution

The 'Dance' song adapted and used in the giveaway animation is courtesy of

*LiQWYD:*

https://www.soundcloud.com/liqwyd

https://spoti.fi/2RPd66h

https://apple.co/2TZtpeG

https://www.instagram.com/liqwyd

https://www.facebook.com/LiQWYD


https://creativecommons.org/licenses/by/3.0/

Music provided by RFM: https://youtu.be/9QDADYhEzXg


## Deployment TODO:

 - [x] Some content on the home page
 - [ ] Need some design help!  Please... Jeff is an art amateur
 - [x] Remove the About link and route it somewhere 
 - [x] Remove the claims on the home page when in Production environment
 - [ ] Join channel feature -- when and how does the bot join and monitor channels
 - [x] Stop monitoring a channel feature -- stop the Follower / Sub / Cheer notifications
 - [ ] Improve the configuration screen
    - [x] Activate the bot
    - [ ] Activate individual features


 ## After Deployment
 - [ ] How to build and test if you are not actively streaming?



## Possible solutions to Blazor disconnect
 >> Persist data between restarts using the StateController and an out-of-process / out-of-container storage service
 1. Extend the reconnect timeout
      - Prevent the screen from going grey
      - We still lose the in-memory state of the application (THIS IS A PROBLEM)
 1. Deploy to a staging slot
      - Swap slot when the application is running
      - We still lose the in-memory state of the application (THIS IS A PROBLEM)
 1. Make the widget static HTML with JavaScript / Blazor WASM
      - This might be the BEST solution
 







