using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace PortBeaconWindowsService
{
    public class PortMonitor
    {
        private List<IBeaconServer> servers = new List<IBeaconServer>();
        private ConfigLoader config;
        private Timer recheckTimer;

        public void Start()
        {
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
            string configPath = System.IO.Path.Combine(exeFolder, "config.txt");

            Logger.Log($"Attempting to load configuration file from: {configPath}");

            config = ConfigLoader.Load(configPath);

            Logger.Configure(config.EnableEventLog, config.EnableFileLog);

            var configSummary = $@"
----- Configuration Summary -----
EnableEventLog: {config.EnableEventLog}
EnableFileLog: {config.EnableFileLog}
EnableDebugLog: {config.EnableDebugLog}
PortCheckMethod: {config.PortCheckMethod}
RecheckIntervalSeconds: {config.RecheckIntervalSeconds}
MaxLogFiles: {config.MaxLogFiles}
Version: {config.Version}
Port Mappings loaded: {config.PortMappings.Count}
----------------------------------";

            Logger.Log(configSummary.Trim());

            foreach (var mapping in config.PortMappings)
            {
                try
                {
                    bool isListening = config.PortCheckMethod.Equals("Netstat", StringComparison.OrdinalIgnoreCase)
                        ? IsPortListeningByNetstat(mapping.LocalPort)
                        : IsPortListeningByConnect(mapping.LocalPort);

                    if (config.EnableDebugLog)
                    {
                        Logger.Log($"[DEBUG] Checking LocalPort {mapping.LocalPort}: {(isListening ? "LISTENING" : "NOT listening")}");
                    }

                    if (!isListening)
                    {
                        if (config.EnableDebugLog)
                            Logger.Log($"[DEBUG] Skipping BeaconPort {mapping.BeaconPort} because LocalPort {mapping.LocalPort} is not listening.");
                        continue;
                    }

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

            if (config.RecheckIntervalSeconds > 0)
            {
                recheckTimer?.Dispose();
                recheckTimer = new Timer(RecheckPorts, null, config.RecheckIntervalSeconds * 1000, config.RecheckIntervalSeconds * 1000);
                Logger.Log($"Rechecking ports every {config.RecheckIntervalSeconds} seconds.");
            }
            else
            {
                Logger.Log("Port rechecking disabled (interval = 0).");
            }
        }

        public void Stop()
        {
            if (config.EnableDebugLog)
            {
                Logger.Log("[DEBUG] Stopping all active Beacon servers...");
            }

            recheckTimer?.Dispose();
            recheckTimer = null;

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

        private void RecheckPorts(object state)
        {
            if (config.EnableDebugLog)
            {
                Logger.Log("[DEBUG] Rechecking port mappings triggered...");
            }

            try
            {
                Stop();
                Start();
            }
            catch (Exception ex)
            {
                Logger.Log("Error during recheck: " + ex.Message);
            }
        }

        private bool IsPortListeningByConnect(int port)
        {
            try
            {
                using (var client = new System.Net.Sockets.TcpClient())
                {
                    var result = client.BeginConnect("127.0.0.1", port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(200));
                    return success && client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool IsPortListeningByNetstat(int port)
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            var listeners = ipProperties.GetActiveTcpListeners();
            return listeners.Any(ep => ep.Port == port);
        }
    }
}
