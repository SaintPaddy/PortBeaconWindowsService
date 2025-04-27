@echo off
sc stop PortBeaconService
sc delete PortBeaconService

sc create PortBeaconService binPath= "%~dp0WindowsServicePortBeacon.exe" start= auto
sc description PortBeaconService "Port Beacon Service v1.0 - Created by Patrick - https://github.com/SaintPaddy/WindowsServicePortBeacon"
sc start PortBeaconService

echo.
echo PortBeaconService installed and started successfully.
pause
