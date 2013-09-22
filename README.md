DayZ DB Access
==============

(WIP) Simple tool to show some data from a DayZ database.

 - Supporting classic DayZ databases. (Basic access to the Epoch's specific DB)
 - Show on map:
	- players online *
	- players alive
	- vehicles - can be filtered by types
	- vehicle spawn points - can be filtered by types **
	- deployables - can be filtered by types
 - Show players/vehicles/deployables inventory.
 - Bitmap selection for each World's type.
 - Map helper to do the link between selected bitmap & DB's coordinates
 - Vehicles Tab: associate an icon to each vehicle's class.
 - Deployables Tab: associate an icon to each deployable's class.
 - Scripts Tab:
	- backup database
	- remove destroyed vehicles **
	- spawn new vehicles **
	- remove old bodies **
	- remove old tents **
	- 3 custom SQL or BAT scripts can be set & called.

 *  : not yet available with Epoch's DB.
 ** : disabled with Epoch's DB because useless.


Executable (ClickOnce) can be found here:

http://82.67.37.43/publish


History
=======

- v2.5.0
	added checkboxes to filter displayed deployables/vehicles types.

- v2.4.0
	added button to backup the database.
	added 3 buttons to call custom SQL files, or BAT files.

- v2.3.0
	added separate definition of Map helpers - Only Chernarus & Celle2 are done
	redone the Map helper for Chernarus, now a grid + NWAF + Skalisty island

- v2.2.0
	added customizable deployables Tab.

- v2.1.0
	new zooming system, removing the use of slow bicubic interpolation.
	fixed some user interactions when using Map helper

- v2.0.0
	added support for Epoch database (not yet fully functional)
	added graphical helper to adapt DB coordinates to selected bitmap

- v1.7.0
	changed for a fully custom display:
		- mipmapped tiled display (DayZDB style)
		- icons can be displayed off map.
		- instant refresh of icons.
		- faster display of big map.
		- small memory usage.
		- created when map selection is changed by user.
	spawn points info

- v1.6.1
	modified tent display to add staches.

- v1.6.0
	using new method to store config file, won't be lost when new update is published.
	changed handling of instances & worlds, now using world id from instance.
	bitmaps are set per world id, and reloaded when changed.

- v1.5.0
	added customizable vehicle type per class, with corresponding icon.
	added contextual menu :
		- delete vehicle
		- delete spawnpoint (if no instanciated vehicles for this spawnpoint)

- v1.4.0
	Using a dataset to manage maps data, allowing to define a map per instance id 


- v1.3.5
	Added medical states, hunger & thirst percent in player's property grid
	Added cargo's total count in property grid

- v1.3.0
	Refactored handling of icons, drastically faster and correctly displayed.
	Added trail on players & vehicles (refresh has to be fixed later)
	Refactored property grid, now displayed in order, expanded and auto refreshed.

- v1.2.0
	Refactored "Show Players Online"
	Refactored mySQL queries
	Refactored some code

- v1.1.0
	selection of database and instance id.
	
	custom map display. (can be set up in config.xml, bitmaps not included)
	
	show on map
		alive players
		online players (will be refactored soon)
		valid and destroyed vehicles
		vehicle spawn points
		tents
	
	show data and inventory from selected player, vehicle or tent
	
	commands:
		Remove destroyed vehicles
		Spawn vehicles
		Remove bodies older than X days
		Remove tents not updated since X days
