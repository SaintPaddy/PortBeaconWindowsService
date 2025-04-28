using System;
using System.Collections.Generic;
using System.Threading;

namespace PortBeaconWindowsService
{
    public class PortMonitor
    {
        private List<IBeaconServer> servers = new List<IBeaconServer>();
        private ConfigLoader config;

        public void Start()
        {
            config = ConfigLoader.Load("config.txt");

            Logger.Configure(config.EnableEventLog, config.EnableFileLog);

            foreach (var mapping in config.PortMappings)
            {
                try
                {
                    if (mapping.Protocol.Equals("TCP", StringComparison.OrdinalIgnoreCase))
                    {
                        var server = new TcpBeaconServer(mapping.LocalPort, mapping.BeaconPort);
                        server.Start();
                        servers.Add(server);
                        Logger.Log($"Monitoring TCP {mapping.LocalPort} -> {mapping.BeaconPort}");
                    }
                    else if (mapping.Protocol.Equals("UDP", StringComparison.OrdinalIgnoreCase))
                    {
                        var server = new UdpBeaconServer(mapping.LocalPort, mapping.BeaconPort);
                        server.Start();
                        servers.Add(server);
                        Logger.Log($"Monitoring UDP {mapping.LocalPort} -> {mapping.BeaconPort}");
                    }
                    else
                    {
                        Logger.Log($"Unknown protocol '{mapping.Protocol}' for port {mapping.LocalPort}. Skipping.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error starting server for {mapping.LocalPort}: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            foreach (var server in servers)
            {
                try
                {
                    server.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Log("Error stopping server: " + ex.Message);
                }
            }
            servers.Clear();
        }
    }
}
