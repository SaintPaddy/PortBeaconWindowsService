using System;
using System.ServiceProcess;
using System.Threading;

namespace PortBeaconWindowsService
{
    public class PortBeaconService : ServiceBase
    {
        private PortMonitor monitor;

        public PortBeaconService()
        {
            ServiceName = "PortBeaconService";
        }

        protected override void OnStart(string[] args)
        {
            Logger.Log("PortBeaconService v1.0 - Created by Patrick - https://github.com/SaintPaddy/PortBeaconWindowsService");

            try
            {
                monitor = new PortMonitor();
                monitor.Start();
                Logger.Log("Service started.");
            }
            catch (Exception ex)
            {
                Logger.Log("Error during service start: " + ex.Message);
                Stop();
            }
        }

        protected override void OnStop()
        {
            try
            {
                monitor?.Stop();
                Logger.Log("Service stopped.");
            }
            catch (Exception ex)
            {
                Logger.Log("Error during service stop: " + ex.Message);
            }
        }
    }
}
