DayZ DB Access
==============

Simple tool to show some data from a DayZ database.

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
