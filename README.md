### FPS_I_GUESS v2.1.2
Full remake of the game from scratch to improve upon various issues, as well as updating the unity version to 2021.3.19f1 to allow the addition of client-server multiplayer.

version 2.1 - improves many of the existing scripts with new methods for better functionality and more efficient code.
version 2.1.2 - Updated the weapon system and interact system to incorporate SOLID principles and to reduce code.

Use the project page to see the current progress: https://github.com/users/RWilko31/projects/1

![image](https://github.com/RWilko31/FPS_I_GUESS/blob/FPS_I_GUESS-v2.1.2/game3.png)

### New additions to the game for 2.1.2:
**Interact system**
- Full redesign from the ground up
- Changed interact detection to use dot product and thresholding to calculate which object is being looked at
- Changed code to use SOLID principles which greatly reduces the amount of code executed
- Implemented an interact interface. This means each object can use its own unique script instead of sharing the same across all objects

**Weapon system**
- Full redesign maintaining the original functionality
- Greatly increased reusablity by implementing multiple inhertance classes. There are now generic Weapon classes that define parameters for any weapon, and attack classes that define the attack performed by the weapon. The attack class is attached to the weapon class allowing it to be freely swapped out for other attacks during runtime. This allows full customisability between melee and ranged weapons as well as upgrading by swapping the attacks
 
**Character movement**
-Implemented basic step-offset allowing the player to climb up and down stairs without sloped colliders underneath. This is still in development and is currently quite jittery

**Input sytem**
-Created events for inputs where applicable. This means scripts no longer need to be referenced by the Input system to be triggered instead each subscribing to the events and being triggered automatically.
-Created stsatic variables for remaining inputs (move input, look input) to prevent the need to refernce the input manager on scripts not requiring it otherwise.

------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

### New additions to the game for v2.1.:
Input:
- input has now been given its own script to avoid each script requiring its own input components and having to switch control schemes in multiple scripts

**Player:**
- grappling hook physics has been improved to allow continued momentum when releasing the grappling hook
- added spawnpoint and checkpoints objects which can be easily placed in the editor 

**Enemies**
- Enemies now have spawn points and respawn points! no more restarting when everything is beaten

**Health system**
- Health system now uses the healthTable.csv file to get health data allowing for easy changes

**Damage system**
- When an entity is damaged in game an attack name will be sent allowing different types of attacks from the same enemy
- inclusion of damageTable.csv to allow adding new attacks or changing damage easily

**Items**
- Item system is now in place, and enemies will give drops when killed which can be used to craft items
- Inclusion of a itemTable.csv file to easily add items to the game

**UI:**
- added player Inventory with accessory slots for equipment. This allows the player to select an item and equip it although no effects are yet given
- crafting interface has been added and a temporary game object to represent it

**Save data:**
- fixed issues with options. the dropdown options now show correct text when cancelling changes

### Notes:
Multiplayer has been put on hold due to the inability to fully test online play due to issues with port forwarding. The current state will remain and be improved at a later date when the base game is more complete.
The main focus has shifted to fully incorporating the item system and play equipment, and subsequently adding in damage and defense stats. After this effort will be put into polishing the appearance of the game by creating more models adding some levels and looking into learning to incorporate textures, materials, particle effects and lighting.

