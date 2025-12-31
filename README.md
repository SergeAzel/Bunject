# Bunject
Bunject Paquerette Modding

There are six projects in this:

## 1. Bunject
   
This project contains nearly all of the Harmony patches used to support the other projects / mods in this solution.
Purpose is to expose convenient endpoints that modding support can be added to.
Generally not intended to contain "mod-specific" logic on its own, but this is not always strictly followed.

Defines the IBunjector interface that other mods use to interact with the game.
Multiple other interfaces derive from IBunjector, to provide various extension points.
Supplies the BunjectAPI endpoint which other mods should register their bunjectors and custom burrows with.

Should really be given a full makeover at some point soon.


## 2. BunjectNewYardSystem
   
BNYS mod - for creating custom burrows.
Handles all of the processes of importing and interpreting custom burrows and custom levels.
Generally not intended to do any direct hacking of things, but regularly uses the Harmony Traverse tools to create custom game objects.


## 3. BunjectArchipelago

Archipelago support for Paquerette Down The Bunburrows.  Dependent upon Bunject.

The APWorld for this mod can be found in the following repository:
https://github.com/SergeAzel/Archipelago_PDTB


## 4. BunjectComputer

This relatively small dll is used to enable the computer map within custom bunburrows.
Honestly an extremely solid improvement, created by StartUp.



# Deprecated
The following two projects should be considered deprecated:

## 5. BunjectExtractor
   
Will save .level files of CORE game levels to an EXTRACTED folder.  Only intended for reading how core levels do things if absolutely needed.

## 6. SaveFileCleanup
   
Used to clean up any save files from any knowledge of custom burrows and custom levels, in case a save file gets horribly fucked up.
If used WITH bunject, will affect your MOD save files.
If used WITHOUT bunject, will affect your CORE save files.
Cannot be used to fix bunnies that were added to modified core levels.
