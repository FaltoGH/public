using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace NaverNews
{
    public class News
    {
        public string Title;
        public string Href;
        public string Time;


        public static implicit operator News(HtmlNode newsNode)
        {
            News news = new News();
            HtmlNode titleNode = newsNode.SelectSingleNode("div[2]/div/a[1]/span");
            string titleText = titleNode.InnerText;
            news.Title = WebUtility.HtmlDecode(titleText);
            HtmlNode hrefNode = newsNode.SelectSingleNode("div[2]/div/a[1]");
            string hrefValue = hrefNode.GetAttributeValue("href", null);
            news.Href = WebUtility.HtmlDecode(hrefValue);
            HtmlNode timeNode = newsNode.SelectSingleNode("div[1]/div[1]/div[3]/span[contains(@class,'text')]/div/span[not(contains(text(),'면'))]");
            string timeText = timeNode.InnerText;
            news.Time = WebUtility.HtmlDecode(timeText);
            return news;
        }

        public static News[] FromHtmlDocument(HtmlDocument htmlDocument)
        {
            HtmlNode documentNode = htmlDocument.DocumentNode;
            HtmlNodeCollection newsNodes = documentNode.SelectNodes("/html/body/div[3]/div[2]/div[1]/div[1]/section[contains(@class,'news')]/div[1]/div[2]/ul/div/div/div/div/div");
            return newsNodes.Select(x=>(News)x).ToArray();
        }


        /// <param name="query">e.g., 삼성전자, F&amp;F, S-Oil, THE CUBE&amp;, 이재멍</param>
        public static News[] FromQuery(string query)
        {
            string urlEncodedQuery = HttpUtility.UrlEncode(query);
            string queryUrl = "https://search.naver.com/search.naver?where=news&query=" + urlEncodedQuery;
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = htmlWeb.Load(queryUrl, "GET");
            return FromHtmlDocument(htmlDocument);
        }

        public override string ToString()
        {
            return $"{Title} ; {Time} ; {Href}";
        }
    }
}
