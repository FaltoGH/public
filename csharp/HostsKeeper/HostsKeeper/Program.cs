using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HostsKeeper
{
    internal static class Program
    {
        private const bool START_NEW_PROCESS = true;

        private static void Cooldown()
        {
            Thread.Sleep(3333);
        }


        private static void KeepHosts(Process hostsKeeperKeeper = null)
        {
            if (START_NEW_PROCESS)
            {
                new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            if (hostsKeeperKeeper is null)
                            {
                                Console.WriteLine("Starting " + Assembly.GetExecutingAssembly().Location);
                                hostsKeeperKeeper = Process.Start(
    Assembly.GetExecutingAssembly().Location, Process.GetCurrentProcess().Id.ToString());
                            }
                            hostsKeeperKeeper.WaitForExit();
                            hostsKeeperKeeper.Dispose();
                            hostsKeeperKeeper = null;
                            Cooldown();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("catched thread exception: " + ex);
                    }
                }).Start();
            }

            string hosts = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");

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
                            File.WriteAllText("C:\\Windows\\System32\\drivers\\etc\\hosts", hosts);
                        }
                        break;
                }

                Cooldown();

            }
        }

        private static void KeepHostsKeeper(int processId)
        {
            Process process = Process.GetProcessById(processId);
            while (true)
            {
                process.WaitForExit();
                process.Dispose();
                process = null;
                Cooldown();
                process = Process.Start(
Assembly.GetExecutingAssembly().Location, "a" + Process.GetCurrentProcess().Id);
            }
        }


        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(
$"Program is starting... pid={Process.GetCurrentProcess().Id} args=[{string.Join(", ", args)}]");
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
                        int processId = int.Parse(arg0.Substring(1));
                        Process process = Process.GetProcessById(processId);
                        KeepHosts(process);
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
                Console.WriteLine("catched exception: " + ex);
            }

            Console.WriteLine("Program is terminating...");
            Thread.Sleep(10000);
        }

    }
}

