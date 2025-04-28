using System;

namespace PortBeaconWindowsService
{
    public static class Utilities
    {
        public static string Timestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
