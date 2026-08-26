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
                int port;
                
                if (args.Length < 1)
                {
                    port = 3000;
                }
                else
                {
                    port = int.Parse(args[0]);
                }

                var listener = new HttpListener();
                listener.Prefixes.Add($"http://localhost:{port}/");
                listener.Start();
                Console.WriteLine($"Now listening on port number {port}");

                //var stdout = Console.OpenStandardOutput();
                //byte[] buffer = new byte[1024];

                while (true)
                {
                    var ctx = listener.GetContext();
                    var resp = ctx.Response;
                    resp.StatusCode = (int)HttpStatusCode.OK;
                    resp.StatusDescription = "Status OK";
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
