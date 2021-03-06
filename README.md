DayZ DB Access
==============
![Screenshot](http://ddba.olympe.in/ShowVehicles.jpg "Screenshot")

(WIP) Simple tool to show some data from a DayZ database. (Windows Only)

 - Supporting classic & Epoch DayZ databases.
 - Show on map:
	- players online/alive/dead
	- vehicles
	- vehicle spawn points
	- deployables/objects
 - Show connected players & rCon admins
 - Filter items by class
 - Filter items by time
 - Show players/vehicles/objects inventory
 - Add/Delete vehicle instances / spawnpoints
 - Revive/Heal player
 - Save/Restore player inventory & backpack
 - Teleport offline players/vehicles
 - PM/Kick player
 - Bitmap selection for each World's type.
 - Map helper to do the link between selected bitmap & DB's coordinates
 - Vehicles Tab: associate an icon to each vehicle's class.
 - Objects Tab: associate an icon to each object's class.
 - Scripts Tab:
	- backup database
	- remove destroyed vehicles
	- spawn new vehicles (1)
	- remove old bodies
	- remove old tents (1)
	- 3 custom SQL or BAT scripts can be set & called.
 - Setup Tab:
 	- refresh rates for Database and BattlEye
 	- set admin name
 	- choose epoch world 

(1) : disabled with Epoch's DB because useless.

Executable (ClickOnce) can be found here:
http://ddba.olympe.in/publish.htm

Chernarus 16384*13824 bitmap can be found here:
http://ddba.olympe.in/Chernarus_16k.jpg

Configuration
=============

 - Select the connection settings for your database.
 - Select the bitmap you want to use, try to use a big one. (no bitmap included)
 - Set icons for each type of vehicles and deployables.

Help
====

 - Click on icons on the map to see details in the Display Tab.
 - Clicking on an icon will select its type in the corresponding vehicle/deploayble's Tab.
 - Contextual menu on vehicle and spawn icons to remove them from the database.
 - Contextual menu on spawn icons to add a new one into the database.
 - Scripts Tab: 3 buttons can be set up to call custom SQL and BAT files.

If something goes wrong, all files created by this application are stored in your %appdata%\DayZDBAccess directory.

You can edit the config file, or delete all files to restore an empty configuration file.

=============
Using the BattleNET library from Marcel de Vries / Robert van der Boorn
https://github.com/dddlazer/BattleNET

Some icons used in this application have been borrowed from DayZDB and DayZST web sites, thanks to them even if I didn't asked permission for that... :S
