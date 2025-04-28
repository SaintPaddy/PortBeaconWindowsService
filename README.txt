Windows Service :: Port Beacon
=========================

Created by Patrick  
https://github.com/SaintPaddy/PortBeaconWindowsService

🛠 Feel free to check/edit the source code and compile the executable yourself in Visual Studio,  
or download the provided MSI file to install it quickly into `C:\ProgramData\`.

---

Overview:
---------
**PortBeaconWindowsService** is a lightweight Windows Service designed to safely support network scanning across segregated VLANs.

It monitors specified local TCP or UDP ports, and if a service is detected as active (for example RDP or SMB), it dynamically opens a corresponding **Beacon port**.

Scanning tools can then detect these safe Beacon ports instead of connecting directly to the real services — reducing security risks while still confirming system availability.

All monitored ports and Beacon mappings are fully configurable through a simple text file.

**Example:**  
You want to know if TCP port 445 (SMB) is open on servers in secure VLANs, but you don't want to allow direct SMB access through your firewall.  
You configure this service to check if TCP-445 is open. If it is, the service opens a Beacon port like TCP-12445.  
Scanning for TCP-12445 then tells you whether the original service was active — safely and without exposing sensitive ports.

The service listens on Beacon ports and performs real TCP handshakes, but does not serve any data.  
It only accepts a connection, then closes it immediately.

---

Features:
---------
- Monitor multiple TCP and UDP ports (where UDP is a bit more tricky; works best with TCP ports).
- Open safe Beacon ports dynamically when local services are active.
- Recheck local port status at configurable intervals.
- Two local port checking methods:
  - **Connect**: Real TCP connection (timeout 200ms).
  - **Netstat**: Read system listening ports without connecting.
- Rotating local log files (default 5MB per file, configurable retention).
- Debug logging mode (optional, very detailed internal tracing).
- Logging to Windows Event Log and/or local `service.log` file.
- Safe error handling, designed to run unattended for long periods.

---

Requirements:
--------------
- Windows Server 2008 R2 or newer (Windows 10/11 also supported).
- .NET Framework 4.5 or higher must be installed.

**Note:**  
If .NET Framework 4.5+ is not present, installation will fail or the service will not start.  
You can install .NET 4.5 manually from Microsoft or enable it via Windows Features / Server Manager.

---

Installation:
-------------
You have two options:

#### Option 1: Install using the MSI package (recommended)

1. Download the latest `.msi` file from the [Releases page](https://github.com/SaintPaddy/PortBeaconWindowsService/releases).
2. Run the `.msi` installer.
3. The service will be installed automatically into `C:\ProgramData\PortBeaconWindowsService\`.
4. A default `config.txt` will be created if not already present.
5. The service will be installed and started automatically as **PortBeaconService**.

*(This is the fastest and easiest way to install.)*

---

#### Option 2: Build manually from source code

1. Clone or download this repository.
2. Open the solution in Visual Studio 2019 or newer.
3. Build the project in **Release** mode.
4. Copy the output files (`PortBeaconWindowsService.exe`, `install_service.bat`, `uninstall_service.bat`, `config.txt`, and icon if needed) into your desired installation folder.
5. Run `install_service.bat` as Administrator to install and start the service.
6. If `config.txt` is missing, the service will auto-generate a default configuration at startup.

*(This option allows full control if you want to customize the code.)*

---

Settings Explained:
--------------------
- **EnableEventLog**: true/false  
  Enables logging to Windows Event Viewer (Application log).

- **EnableFileLog**: true/false  
  Enables logging to a local `service.log` file (auto-rotates after 5MB).

- **EnableDebugLog**: true/false  
  Enables detailed debug-level logging.  
  In debug mode:
  - Port status checks are logged at every recheck.
  - Skipped ports are logged.
  - Log rotation errors are logged.
  - Recheck timer events are logged.
  
  Without debug mode, only important events (startup, shutdown, errors) are logged.

- **PortCheckMethod**: Connect / Netstat  
  - **Connect**: Attempts a real TCP connection to the LocalPort (200ms timeout).
  - **Netstat**: Checks if the LocalPort is listening via the OS socket table (no actual connection).

- **RecheckIntervalSeconds**: Number  
  - `0` = Disable automatic rechecking after startup.
  - `>0` = Recheck local port availability every X seconds.

- **MaxLogFiles**: Number  
  - Limits the number of rotated archived log files (`service-YYYYMMDD-HHMMSS.log`).
  - Older logs beyond this limit are automatically deleted.
  - `0` = Keep unlimited old logs.

- **Version**: Informational version string.

**Note:**  
The service reads `config.txt` **only at startup**.  
You must restart the service if you change the configuration.

---

Event Viewer:
-------------
- Log Type: **Application**
- Event Source: **PortBeaconService**
- You can filter logs by "PortBeaconService" for easy viewing.

---

Uninstallation:
-------------
You have two options:

#### Option 1: Uninstall using the MSI package

1. Go to the Control Panel / Add Remove Programs / etc to uninstall the software. 

---

#### Option 2: If build manually from source code

1. Run `uninstall_service.bat`.
2. Optionally, manually delete the installation folder.

---

License:
--------
MIT License.  
Feel free to use, modify, and redistribute.  
Credit appreciated!

Project page: [https://github.com/SaintPaddy/PortBeaconWindowsService](https://github.com/SaintPaddy/PortBeaconWindowsService)

