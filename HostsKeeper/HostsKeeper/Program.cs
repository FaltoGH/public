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
        private const string HOSTS = "C:\\Windows\\System32\\drivers\\etc\\hosts";
        private static readonly int PID = Process.GetCurrentProcess().Id;


        private static void Cooldown()
        {
            Thread.Sleep(1);
        }


        private static string NewProgramLocation()
        {
            if (!File.Exists("HostsKeeper.exe"))
            {
                throw new FileNotFoundException();
            }

            return "HostsKeeper.exe";

            //string[] files = Directory.GetFiles(".", "*.exe");
            //foreach (var file in files)
            //{
            //    if (Path.GetFileNameWithoutExtension(file) == "HostsKeeper")
            //    {
            //        continue;
            //    }
            //    try
            //    {
            //        File.Delete(file);
            //    }
            //    catch (System.UnauthorizedAccessException)
            //    {
            //    }
            //}
            //string destFileName = Guid.NewGuid().ToString() + ".exe";
            //File.Copy("HostsKeeper.exe", destFileName);
            //return destFileName;
        }


        private static void KeepHosts(Process hostsKeeperKeeper = null)
        {
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if(hostsKeeperKeeper is null)
                        {
                            string newProgramLocation = NewProgramLocation();
                            hostsKeeperKeeper = Process.Start(newProgramLocation, PID.ToString());
                        }
                        hostsKeeperKeeper.WaitForExit();
                        hostsKeeperKeeper.Dispose();
                        hostsKeeperKeeper = null;
                        Cooldown();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("catched thread exception: " + ex);
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
                string newProgramLocation = NewProgramLocation();
                process = Process.Start(newProgramLocation, "a" + PID);
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

