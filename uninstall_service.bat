@echo off
sc stop PortBeaconService
sc delete PortBeaconService

echo.
echo PortBeaconService uninstalled successfully.
pause
