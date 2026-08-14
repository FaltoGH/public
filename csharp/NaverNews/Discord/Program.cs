using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Discord
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: ./Discord <Discord webhook URI> <executable file name>");
                return;
            }

            string requestUri = args[0];
            string fileName = args[1];
            string output;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
                int exitCode = process.ExitCode;
                if (exitCode != 0)
                {
                    Console.Error.WriteLine($"error: process exit code is {exitCode}");
                    return;
                }
                output = process.StandardOutput.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                Console.Error.WriteLine($"error: output is white space");
                return;
            }

            HttpClient httpClient = new HttpClient();

            if (output.Contains("```"))
            {
            }
            else
            {
                output = "```\r\n" + output + "\r\n```";
            }

            Dictionary<string, string> values = new Dictionary<string, string>
            {
                ["content"] = output
            };
            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(values);
            HttpResponseMessage httpResponseMessage = httpClient.PostAsync(requestUri, formUrlEncodedContent).Result;
            string responseString = httpResponseMessage.Content.ReadAsStringAsync().Result;
            Console.WriteLine(responseString);
        }
    }
}
