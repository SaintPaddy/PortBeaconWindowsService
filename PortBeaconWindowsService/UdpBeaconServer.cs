using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PortBeaconWindowsService
{
    public class UdpBeaconServer : IBeaconServer
    {
        private UdpClient udpClient;
        private Thread listenThread;
        private bool running = false;
        private int monitoredPort;
        private int beaconPort;

        public UdpBeaconServer(int monitoredPort, int beaconPort)
        {
            this.monitoredPort = monitoredPort;
            this.beaconPort = beaconPort;
        }

        public void Start()
        {
            udpClient = new UdpClient(beaconPort);
            running = true;

            listenThread = new Thread(ListenLoop);
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public void Stop()
        {
            running = false;
            udpClient?.Close();
            listenThread?.Join();
        }

        private void ListenLoop()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            while (running)
            {
                try
                {
                    if (udpClient.Available > 0)
                    {
                        byte[] data = udpClient.Receive(ref remoteEP);
                        if (data != null && data.Length > 0)
                        {
                            byte[] reply = Encoding.ASCII.GetBytes(
                                $"PortBeaconService v1.0 - Port UDP {monitoredPort} Active\r\nCreated by Patrick - https://github.com/SaintPaddy/PortBeaconWindowsService\r\n"
                            );
                            udpClient.Send(reply, reply.Length, remoteEP);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                catch (SocketException)
                {
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Logger.Log($"UDP ListenLoop Error: {ex.Message}");
                    Thread.Sleep(500);
                }
            }
        }
    }
}
