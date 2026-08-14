using System.Diagnostics;

namespace FileWatch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("usage: ./FileWatch <path> <WatcherChangeTypes> <Discord webhook URI>\r\nenum WatcherChangeTypes{Created=1,Deleted=2,Changed=4,Renamed=8,All=15}");
                    return;
                }

                string path = args[0];

                if (!Directory.Exists(path))
                {
                    Console.WriteLine("error: Directory does not exist. Path is " + path);
                    return;
                }

                Console.WriteLine(Directory.GetFiles(path).Length + " files currently exists.");

                WatcherChangeTypes watcherChangeTypes = (WatcherChangeTypes)byte.Parse(args[1]);
                string requestUri = args[2];

                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(path);

                HttpClient httpClient = new HttpClient();
                Stopwatch stopwatch = Stopwatch.StartNew();

                for (int i = 1; i < 100; i++)
                {
                    WaitForChangedResult waitForChangedResult = fileSystemWatcher.WaitForChanged(watcherChangeTypes);

                    if (stopwatch.ElapsedMilliseconds > 10000)
                    {
                        string content = $"{waitForChangedResult.ChangeType} {waitForChangedResult.Name}";
                        Dictionary<string, string> values = new Dictionary<string, string>
                        {
                            ["content"] = content
                        };
                        FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(values);
                        HttpResponseMessage httpResponseMessage = httpClient.PostAsync(requestUri, formUrlEncodedContent).Result;
                        string responseString = httpResponseMessage.Content.ReadAsStringAsync().Result;
                        stopwatch.Reset();
                    }

                    Thread.Sleep(1);
                }

                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("catched exception: " + ex);
            }
        }
    }
}
