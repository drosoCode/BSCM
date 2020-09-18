# Beat Saber Co-op Mod - BSCM v1.0.0

A co-op mod for Beat Saber

Based on [BeatSaber-Claws](https://github.com/SteffanDonal/BeatSaber-Claws)
Depends on [websocket-sharp](https://github.com/sta/websocket-sharp) for communication

## Installation

 - In your Beat Saber directory: 
 - Copy BSCM.dll and websocket-sharp.dll in the Plugins directory
 - Copy BSCM.json in the UserData directory

## Configuration

In the BSCM.json file:
 - url is the server url (only for client mode, use the public ip of your teammate in server mode)
 - port is the server port
 - isServer is client or server mode selection (you must set this to true for one user and to false for the other, a port forwarding is mandatory for the user in server mode)
 - isLeftRemoteSaber is the saber controlled by your teammate, if true this is the left saber (you must set this to true for one user and to false for the other)
 - disableSubmission permits to disable score submission when playing with this mod
 
## How to play

 - You control one saber and your teammate controls the other
 - Your must use the same map on the same settings (difficulty and modifiers)
 - Click **Play** when you are ready, the game will start when the 2 players are ready 
