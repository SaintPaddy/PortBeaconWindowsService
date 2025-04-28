using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PortBeaconWindowsService
{
    public class ConfigLoader
    {
        public static ConfigLoader Current { get; private set; }

        public List<PortMapping> PortMappings { get; set; } = new List<PortMapping>();
        public bool EnableEventLog { get; set; } = true;
        public bool EnableFileLog { get; set; } = false;
        public bool EnableDebugLog { get; set; } = false;
        public string PortCheckMethod { get; set; } = "Connect";
        public string Version { get; set; } = "1.0";
        public int RecheckIntervalSeconds { get; set; } = 0;
        public int MaxLogFiles { get; set; } = 10;

        public static ConfigLoader Load(string filePath)
        {
            var config = new ConfigLoader();

            if (!File.Exists(filePath))
            {
                Logger.Log("Configuration file not found. Creating default config.txt...");

                try
                {
                    File.WriteAllText(filePath,
@"# Port Mappings: <LocalPort> <BeaconPort> <Protocol>
3389 33389 TCP
445 33445 TCP
161 3161 UDP

# Settings
EnableEventLog true
EnableFileLog false
EnableDebugLog false
PortCheckMethod Connect
RecheckIntervalSeconds 30
MaxLogFiles 10

# Optional
Version 1.0

# Documentation
https://github.com/SaintPaddy/PortBeaconWindowsService
");
                    Logger.Log("Default configuration file created at: " + filePath);
                }
                catch (Exception ex)
                {
                    Logger.Log("Failed to create default configuration file: " + ex.Message);
                    throw;
                }
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                if (trimmed.StartsWith("EnableEventLog", StringComparison.OrdinalIgnoreCase))
                    config.EnableEventLog = trimmed.ToLower().Contains("true");
                else if (trimmed.StartsWith("EnableFileLog", StringComparison.OrdinalIgnoreCase))
                    config.EnableFileLog = trimmed.ToLower().Contains("true");
                else if (trimmed.StartsWith("EnableDebugLog", StringComparison.OrdinalIgnoreCase))
                    config.EnableDebugLog = trimmed.ToLower().Contains("true");
                else if (trimmed.StartsWith("PortCheckMethod", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = trimmed.Split(' ');
                    if (parts.Length == 2)
                        config.PortCheckMethod = parts[1].Trim();
                }
                else if (trimmed.StartsWith("RecheckIntervalSeconds", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = trimmed.Split(' ');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int seconds))
                        config.RecheckIntervalSeconds = seconds;
                }
                else if (trimmed.StartsWith("MaxLogFiles", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = trimmed.Split(' ');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int maxFiles))
                        config.MaxLogFiles = maxFiles;
                }
                else if (trimmed.StartsWith("Version", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = trimmed.Split(' ');
                    if (parts.Length == 2)
                        config.Version = parts[1];
                }
                else
                {
                    var parts = trimmed.Split(' ');
                    if (parts.Length >= 3 &&
                        int.TryParse(parts[0], out int localPort) &&
                        int.TryParse(parts[1], out int beaconPort))
                    {
                        config.PortMappings.Add(new PortMapping
                        {
                            LocalPort = localPort,
                            BeaconPort = beaconPort,
                            Protocol = parts[2].ToUpper()
                        });
                    }
                }
            }

            // Bonus checks
            var duplicateBeaconPorts = config.PortMappings.GroupBy(p => p.BeaconPort)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateBeaconPorts.Count > 0)
            {
                Logger.Log($"Warning: Duplicate BeaconPorts detected: {string.Join(", ", duplicateBeaconPorts)}");
            }

            var lowLocalPorts = config.PortMappings
                .Where(p => p.LocalPort < 1024)
                .Select(p => p.LocalPort)
                .ToList();

            if (lowLocalPorts.Count > 0)
            {
                Logger.Log($"Warning: LocalPorts below 1024 detected (may require Admin rights): {string.Join(", ", lowLocalPorts)}");
            }

            Current = config; // Set global reference
            return config;
        }
    }

    public class PortMapping
    {
        public int LocalPort { get; set; }
        public int BeaconPort { get; set; }
        public string Protocol { get; set; }
    }
}
