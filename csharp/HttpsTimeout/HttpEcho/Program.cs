using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpEcho
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                var ipEndPoint = new IPEndPoint(IPAddress.Any, 80);
                var listener = new TcpListener(ipEndPoint);
                listener.Start();
                var stdout = Console.OpenStandardOutput();
                while (true)
                {
                    var tcpClient = await listener.AcceptTcpClientAsync();
                    var networkStream = tcpClient.GetStream();
                    byte[] buffer = new byte[1024];
                    int count = 1;
                    while(count > 0)
                    {
                        count = networkStream.Read(buffer, 0, buffer.Length);
                        stdout.Write(buffer, 0, count);
                        Thread.Sleep(1);
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
