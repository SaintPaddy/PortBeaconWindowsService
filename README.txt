Windows Service :: Port Beacon
=========================
Created by Patrick
https://github.com/SaintPaddy/PortBeaconWindowsService


Overview:
---------
PortBeaconWindowsService is a lightweight Windows Service that monitors local ports (TCP or UDP) and opens corresponding "Beacon" ports for scanning tools.

This allows network scanning across VLANs without exposing the real services like RDP, SMB, etc.

The service dynamically listens on configurable Beacon ports if the corresponding local service is running.


Features:
---------
- Monitor multiple TCP and UDP ports.
- Open safe Beacon ports dynamically if local service is active.
- Recheck local port status at configurable intervals.
- Two local port checking methods:
  - **Connect**: Real TCP connection (timeout 200ms).
  - **Netstat**: Read system listening ports without connecting.
- Rotating local log files (default 5MB per file, configurable retention).
- Debug logging mode (optional, very detailed internal tracing).
- Logging to Windows Event Log and/or local service.log file.
- Safe error handling, designed to run unattended for long periods.


Installation:
-------------
1. Build the service project.
2. Install the service using `install_service.bat`.
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
EnableDebugLog false
PortCheckMethod Connect
RecheckIntervalSeconds 30
MaxLogFiles 10

# Optional
Version 1.0

# Documentation
https://github.com/SaintPaddy/PortBeaconWindowsService


Note: The service reads 'config.txt' only at startup. You must restart the service if you change the configuration.


Settings Explained:
--------------------
- **EnableEventLog**: true/false  
  Enables logging to Windows Event Viewer.

- **EnableFileLog**: true/false  
  Enables logging to a local `service.log` file.  
  The log file rotates automatically after 5MB.

- **EnableDebugLog**: true/false  
  Enables detailed debug-level logging.  
  In debug mode:
  - Port status checks are logged every recheck.
  - Skipped ports are logged.
  - Log rotation errors are logged.
  - Recheck timer events are logged.
  
  Without debug mode, only important events (startup, shutdown, errors) are logged.

- **PortCheckMethod**: Connect / Netstat  
  - **Connect**: Attempts real TCP connection to LocalPort (200ms timeout).
  - **Netstat**: Checks if LocalPort is listening based on system socket tables (no connection attempts).

- **RecheckIntervalSeconds**: Number  
  - `0` = Disable automatic rechecking after startup.
  - `>0` = Recheck local port availability every X seconds.

- **MaxLogFiles**: Number  
  - Limits the number of rotated archived log files (`service-YYYYMMDD-HHMMSS.log`).
  - Older logs beyond this limit are automatically deleted.
  - `0` = Keep unlimited old logs.

- **Version**: Informational version string.


Event Viewer:
-------------
- Log Type: Application
- Event Source: PortBeaconService
- Filter for "PortBeaconService" to quickly find service-related logs.


Uninstallation:
---------------
1. Run `uninstall_service.bat`.
2. Optionally, manually delete the service folder if needed.


License:
--------
MIT License.
Feel free to use, modify, and redistribute.  
Credit appreciated!
