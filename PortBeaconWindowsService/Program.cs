using System;
using System.ServiceProcess;

namespace WindowsServicePortBeacon
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PortBeaconService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
