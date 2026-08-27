using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HttpEcho
{
    internal static class Program
    {
        private static readonly Encoding RawTextEncoding = Encoding.Latin1;

        private static void Main(string[] args)
        {
            int port = args.Length > 0 ? int.Parse(args[0]) : 3000;

            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();

            Console.WriteLine($"Listening on http://127.0.0.1:{port}/");

            while (true)
            {
                using Socket socket = listener.AcceptSocket();
                HandleConnection(socket);
            }
        }

        private static void HandleConnection(Socket socket)
        {
            socket.ReceiveTimeout = 500;

            using NetworkStream stream = new NetworkStream(socket, ownsSocket: false);
            using var requestBytes = new MemoryStream();
            var buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        break;
                    }

                    requestBytes.Write(buffer, 0, read);

                    if (socket.Available == 0)
                    {
                        break;
                    }
                }
            }
            catch (IOException)
            {
                // ReceiveTimeout ends the read once the client has finished sending.
            }

            byte[] body = requestBytes.ToArray();
            byte[] headerBytes = RawTextEncoding.GetBytes(
                "HTTP/1.1 200 OK\r\n" +
                "Content-Type: text/plain; charset=latin1\r\n" +
                $"Content-Length: {body.Length}\r\n" +
                "Connection: close\r\n" +
                "\r\n");

            stream.Write(headerBytes, 0, headerBytes.Length);
            stream.Write(body, 0, body.Length);
            stream.Flush();
        }
    }
}
