DayZ DB Access
==============

Simple tool to show some data from a DayZ database.


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
