# Bunject
Bunject Paquerette Modding

There are five projects in this:

1. Bunject
   
This project contains all of the Harmony patches used to support the other projects / mods in this solution.
Purpose is to expose convenient endpoints that modding support can be added to.
Generally not intended to contain "mod-specific" logic on its own, but this is not always strictly followed.

Defines the IBunjector interface that other mods use to interact with the game.
Supplies the BunjectAPI endpoint which other mods should register their bunjectors and custom burrows with.

2. BunjectNewYardSystem
   
BNYS mod - for creating custom burrows.
Handles all of the processes of importing and interpreting custom burrows and custom levels.
Generally not intended to do any direct hacking of things, but regularly uses the Harmony Traverse tools to create custom game objects.

3. BunjectComputer

This relatively small dll is used to enable the computer map within custom bunburrows.
Honestly an extremely strong improvement, created by StartUp.



The following two projects should be considered deprecated:

4. BunjectExtractor
   
Will save .level files of CORE game levels to an EXTRACTED folder.  Only intended for reading how core levels do things if absolutely needed.

5. SaveFileCleanup
   
Used to clean up any save files from any knowledge of custom burrows and custom levels, in case a save file gets horribly fucked up.
If used WITH bunject, will affect your MOD save files.
If used WITHOUT bunject, will affect your CORE save files.
Cannot be used to fix bunnies that were added to modified core levels.
