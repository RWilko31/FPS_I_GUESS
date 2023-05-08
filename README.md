### FPS_I_GUESS v2.1
Full remake of the game from scratch to improve upon various issues, as well as updating the unity version to 2021.3.19f1 to allow the addition of client-server multiplayer.

version 2.1 improves many of the existing scripts with new methods for better functionality and more efficient code.

Use the project page to see the current progress: https://github.com/users/RWilko31/projects/1

![image](https://user-images.githubusercontent.com/92086002/223248146-e062eac8-add3-4389-a1b2-a23d99b1dc63.png)


### New additions to the game for v2.1:
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
Much of the new additions are still work in progress but functional.
the current focus is on multiplayer and afterwards more effort will be put into polishing the appearance of the game by creating more models adding some levels and looking into learning to incorporate textures, materials, particle effects and lighting.

