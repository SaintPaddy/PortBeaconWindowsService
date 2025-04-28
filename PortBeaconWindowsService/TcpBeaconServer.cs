using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WindowsServicePortBeacon
{
    public class TcpBeaconServer : IBeaconServer
    {
        private TcpListener listener;
        private Thread listenThread;
        private bool running = false;
        private int monitoredPort;
        private int beaconPort;

        public TcpBeaconServer(int monitoredPort, int beaconPort)
        {
            this.monitoredPort = monitoredPort;
            this.beaconPort = beaconPort;
        }

        public void Start()
        {
            listener = new TcpListener(IPAddress.Any, beaconPort);
            listener.Start();
            running = true;

            listenThread = new Thread(ListenLoop);
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public void Stop()
        {
            running = false;
            listener?.Stop();
            listenThread?.Join();
        }

        private void ListenLoop()
        {
            while (running)
            {
                try
                {
                    if (!listener.Pending())
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    TcpClient client = listener.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                }
                catch (SocketException)
                {
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    Logger.Log($"TCP ListenLoop Error: {ex.Message}");
                    Thread.Sleep(500);
                }
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = obj as TcpClient;
            if (client == null) return;

            try
            {
                NetworkStream stream = client.GetStream();
                string banner = $"PortBeaconService v1.0 - Port TCP {monitoredPort} Active\r\nCreated by Patrick - https://github.com/SaintPaddy/WindowsServicePortBeacon\r\n";
                byte[] data = Encoding.ASCII.GetBytes(banner);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                Logger.Log($"TCP HandleClient Error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
