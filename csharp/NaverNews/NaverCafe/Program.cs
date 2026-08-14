using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NaverCafe
{
    internal class Program
    {

        private static async Task Main(string[] args)
        {
            Notice[] notices = await Notice.GetNoticesAsync();
            foreach(var notice in notices)
            {
                Console.WriteLine(notice);
            }
        }
    }
}
