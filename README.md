# Beat Saber Co-op Mod - BSCM v1.0.0

A co-op mod for Beat Saber

Based on [BeatSaber-Claws](https://github.com/SteffanDonal/BeatSaber-Claws). Depends on [lidgren-network](https://github.com/lidgren/lidgren-network-gen3) for communication

## Installation

 - Requirements: BSIPA, BS_Utils, BSML
 - Download latest release [here](https://github.com/drosoCode/BSCM/releases)
 - Extract the zip in your Beat Saber directory
 - Edit the BSCM.json file in the UserData directory

## Configuration

In the BSCM.json file:
 - url is the server url (only for client mode, use the public ip of your teammate in server mode)
 - port is the server port (if you are in server mode, you must setup a port forwarding on your router)
 - isServer is client or server mode selection (you must set this to true for one user and to false for the other, a port forwarding is mandatory for the user in server mode)
 - isLeftRemoteSaber is the saber controlled by your teammate, if true this is the left saber (you must set this to true for one user and to false for the other)
 - disableRumble permits to disable rumble on the remote saber
 
## How to play

 - You control one saber and your teammate controls the other
 - Your must use the same map on the same settings (difficulty and modifiers)
 - Click **Play** when you are ready, the game will start when the 2 players are ready 
