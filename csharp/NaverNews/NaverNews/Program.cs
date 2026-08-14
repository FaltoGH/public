using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace NaverNews
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            News[] news = News.FromQuery("삼성전자");
            string result = string.Join("\r\n", news.Select(x => x.ToString()));
            Console.WriteLine(result);
        }
    }
}
