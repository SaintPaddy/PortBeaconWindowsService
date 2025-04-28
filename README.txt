Windows Service :: Port Beacon
=========================

PortBeaconWindowsService
-------------------------

Created by Patrick
https://github.com/SaintPaddy/PortBeaconWindowsService

Overview:
---------
PortBeaconWindowsService is a lightweight Windows Service that monitors local ports (TCP or UDP) and opens corresponding "Beacon" ports for scanning tools.

This allows network scanning across VLANs without exposing the real services like RDP or SMB.


Features:
---------
- Monitor multiple TCP and UDP ports.
- Open safe Beacon ports dynamically.
- Recheck local port status at configurable intervals.
- Logging to Windows Event Log and/or local logfile.
- Log file rotation (5 MB max, automatic archiving).
- Auto-recovers from errors.
- Configurable without restarting the server.


Installation:
-------------
1. Build the service project.
2. Install using the provided `install_service.bat`.
3. Place `config.txt` in the service executable folder (not required, a default file will be created upon service start when it's missing).


Configuration (config.txt):
----------------------------
Example:

# Port Mappings: <LocalPort> <BeaconPort> <Protocol>
3389 33389 TCP
445 33445 TCP
161 3161 UDP

# Settings
EnableEventLog true
EnableFileLog false
RecheckIntervalSeconds 30

# Optional
Version 1.0

# Documentation
https://github.com/SaintPaddy/PortBeaconWindowsService

Note: The service reads 'config.txt' only at startup. You must restart the service if you change the configuration.


Settings Explained:
--------------------
- EnableEventLog: true/false  
  Enable logging to Windows Event Viewer.

- EnableFileLog: true/false  
  Enable logging to a local service.log file.  
  (Rotates automatically after 5 MB).

- RecheckIntervalSeconds: Number  
  - `0` = Disable rechecking (only scan at service start).  
  - `>0` = Recheck local ports every X seconds.

- Version: Version string for tracking.


Logging:
---------
- Event logs will appear under Windows Logs -> Application (Source: PortBeaconService).
- If file logging is enabled, logs will be written to 'service.log' in the installation folder.
- Log files rotate automatically when they reach 5MB.


Logging - Event Viewer:
-------------
- Log Type: Application
- Event Source: PortBeaconService
- Filter for "PortBeaconService" to see all events.


Uninstallation:
---------------
1. Run `uninstall_service.bat`.
2. Delete the service folder if needed.
