using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HostsKeeper
{
    internal static class Program
    {
        private const string HOSTS = "C:\\Windows\\System32\\drivers\\etc\\hosts";
        private static readonly string LOCATION = Assembly.GetCallingAssembly().Location;
        private static readonly int PID = Process.GetCurrentProcess().Id;

        private static void KeepHosts(int hostsKeeperKeeperPid = 0)
        {
            new Thread(() =>
            {
                Process hostsKeeperKeeper;
                if (hostsKeeperKeeperPid > 0)
                {
                    hostsKeeperKeeper = Process.GetProcessById(hostsKeeperKeeperPid);
                }
                else
                {
                    hostsKeeperKeeper = Process.Start(LOCATION, PID.ToString());
                }

                while (true)
                {
                    hostsKeeperKeeper.WaitForExit();
                    Thread.Sleep(1000);
                    hostsKeeperKeeper = Process.Start(LOCATION, PID.ToString());
                }

            }).Start();

            string hosts = File.ReadAllText(HOSTS);

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher("C:\\Windows\\System32\\drivers\\etc\\");

            while (true)
            {
                Console.WriteLine("Waiting for changed...");
                WaitForChangedResult waitForChangedResult = fileSystemWatcher.WaitForChanged(WatcherChangeTypes.Changed);

                switch (waitForChangedResult.ChangeType)
                {
                    case WatcherChangeTypes.Changed:
                        if (waitForChangedResult.Name == "hosts")
                        {
                            Process[] processes = Process.GetProcessesByName("notepad++.exe");
                            foreach (var process in processes)
                            {
                                process.Kill();
                            }
                            File.WriteAllText(HOSTS, hosts);
                        }
                        break;
                }

                Thread.Sleep(1000);

            }
        }

        private static void KeepHostsKeeper(int processId)
        {
            Process process = Process.GetProcessById(processId);
            while (true)
            {
                process.WaitForExit();
                Thread.Sleep(1000);
                process = Process.Start(LOCATION, "a" + PID);
            }
        }

        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Program is starting... pid={PID} args=[{string.Join(", ", args)}]");
                Thread.Sleep(1000);

                if (args.Length == 0)
                {
                    KeepHosts();
                }
                else if (args.Length == 1)
                {
                    string arg0 = args[0];
                    if (arg0[0] == 'a')
                    {
                        int hostsKeeperKeeperPid = int.Parse(arg0.Substring(1));
                        KeepHosts(hostsKeeperKeeperPid);
                    }
                    else
                    {
                        int processId = int.Parse(arg0);
                        KeepHostsKeeper(processId);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Program is terminating...");
            Thread.Sleep(1000);
        }

    }
}

