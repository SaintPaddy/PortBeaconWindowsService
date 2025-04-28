using System;
using System.Collections.Generic;
using System.IO;

namespace WindowsServicePortBeacon
{
    public class ConfigLoader
    {
        public List<PortMapping> PortMappings { get; set; } = new List<PortMapping>();
        public bool EnableEventLog { get; set; } = true;
        public bool EnableFileLog { get; set; } = false;
        public string Version { get; set; } = "1.0";

        public static ConfigLoader Load(string filePath)
        {
            var config = new ConfigLoader();

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Configuration file not found: " + filePath);

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue; // Skip empty lines and comments

                if (trimmed.StartsWith("EnableEventLog", StringComparison.OrdinalIgnoreCase))
                {
                    config.EnableEventLog = trimmed.ToLower().Contains("true");
                }
                else if (trimmed.StartsWith("EnableFileLog", StringComparison.OrdinalIgnoreCase))
                {
                    config.EnableFileLog = trimmed.ToLower().Contains("true");
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
