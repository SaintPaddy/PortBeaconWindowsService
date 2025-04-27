WindowsServicePortBeacon
=========================

Created by Patrick
GitHub: https://github.com/SaintPaddy/WindowsServicePortBeacon
Version: v1.0

---

Description:
-------------
WindowsServicePortBeacon is a lightweight Windows Service that monitors local ports 
and signals their availability by opening "beacon ports" over TCP or UDP.

It allows a scanning tool to check specific custom beacon ports without directly 
touching sensitive ports like 3389 (RDP) or 445 (SMB).

---

Installation:
--------------
1. Open a Command Prompt as Administrator.
2. Navigate to the folder where the compiled EXE is located.
3. Run: install_service.bat

This will install and start the Windows Service.

---

Uninstallation:
----------------
1. Open a Command Prompt as Administrator.
2. Navigate to the folder where the EXE is located.
3. Run: uninstall_service.bat

---

Configuration:
---------------
Edit the 'config.txt' file in the service folder.

Format:

    <LocalPort> <BeaconPort> <Protocol>
    (Protocol must be TCP or UDP)

Settings:

    EnableEventLog true
    EnableFileLog false
    Version 1.0

Example:

    3389 33389 TCP
    445 33445 TCP
    161 3161 UDP

- EnableEventLog (true/false): Enable logging to Windows Event Log.
- EnableFileLog (true/false): Enable logging to a local file (service.log).

Note: The service reads 'config.txt' only at startup.
You must restart the service if you change the configuration.

---

Logging:
---------
- Event logs will appear under Windows Logs -> Application (Source: PortBeaconService).
- If file logging is enabled, logs will be written to 'service.log' in the installation folder.
- Log files rotate automatically when they reach 5MB.

---

Credits:
---------
PortBeaconService v1.0 - Created by Patrick
GitHub Repository: https://github.com/SaintPaddy/WindowsServicePortBeacon

