@echo off
sc stop PortBeaconService
sc delete PortBeaconService

sc create PortBeaconService binPath= "%~dp0WindowsServicePortBeacon.exe" start= auto
sc description PortBeaconService "Port Beacon Service v1.0 - https://github.com/SaintPaddy/PortBeaconWindowsService"
sc start PortBeaconService

echo.
echo PortBeaconService installed and started successfully.
pause
