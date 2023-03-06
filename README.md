### FPS_I_GUESS v2.0
Full remake of the game from scratch to improve upon various issues, as well as updating the unity version to 2021.3.19f1 to allow the addition of client-server multiplayer.

### New additions to the game:
Input:
- improved input system (new unity input system)
- multiple controller type support (PS, XBOX, keyboard and mouse)

**Player:**
- improved character movement script
- updated character model with some animations
- added player interacting, player can now pick up some objects and interact with guns to equip them and doors to open them

**Weapons:**
- added more weapons to the game as well as models for them
- Weapon manager script for weapon inventory
- Weapon table .csv file to add future weapons seamlessly to the game
- Player attack script to use different ammo types and different equipment (guns, grenades, grapplehook)

**Enemies**
- Universal Health system/component which can be applied to all players and enemies  (this will soon be updated to use a csv health table)
- added enemies with some simple behaviour (follow the player, keep some distance, hide when attacked)

**UI:**
- added player HUD/overlay, currently showing ammo and health
- added the ability to load different levels
- added main menu
- added pause menu

**Save data:**
- main menu options are now saved between game sessions
- Added save files which currently saves player weapon data (equipment and ammo)
- Added the ability to load and overwrite save data as well as create new save files
- Made save data/system modular so more data can easily be added without modifying current code (e.g adding health, position, level)

**Multiplayer:**
- added client server based multiplayer system, connecting via an IP address (only tested on same router)
- data can now be sent over the server to all clients to update every players position, rotation and animations

**Audio:**
- added temporary Gun sound effects to the game 
- added main menu theme 
- Included some OST tracks triggered by interactables for tests 

**Gamedata:**
- inclusion of a Gamedata file to easily change aspects of the game (models, audio, variables)
- can now use Gamedata to send variables between scripts

### Notes:
Much of the new additions are still work in progress but functional.
the current focus is on multiplayer and afterwards more effort will be put into polishing the appearance of the game by creating more models adding some levels and looking into learning to incorporate textures, materials, particle effects and lighting.

