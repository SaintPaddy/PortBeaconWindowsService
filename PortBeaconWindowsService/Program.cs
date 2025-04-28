using System;
using System.ServiceProcess;

namespace PortBeaconWindowsService
{
    static class Program
    {
        static void Main(string[] args)
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
