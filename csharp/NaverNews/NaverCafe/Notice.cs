using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NaverCafe
{
    public class Notice
    {
        public readonly string Href;
        public readonly string Title;

        public Notice(string href, string title)
        {
            Href = href;
            Title = title;
        }

        public override string ToString()
        {
            return $"{Title} ; {Href} ";
        }

        public static async Task<Notice[]> GetNoticesAsync()
        {
            byte[] byteArray;
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("https://cafe.naver.com/MyCafeIntro.nhn?clubid=31041733");
                byteArray = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding euckr = Encoding.GetEncoding("euc-kr");
            HtmlDocument htmlDocument = new HtmlDocument();
            string html = euckr.GetString(byteArray);
            htmlDocument.LoadHtml(html);
            HtmlNode documentNode = htmlDocument.DocumentNode;
            HtmlNodeCollection titleNodes = documentNode.SelectNodes("//*[@id=\"cafe-data\"]/div[1]/div/div/table/tbody/tr[contains(@class,'notice')]/td[1]/div[2]/div/a");

            if (titleNodes == null)
            {
                titleNodes = documentNode.SelectNodes("/html/body/div[1]/div/div/div/div[3]/div/div[1]/div/div/table/tbody/tr[contains(@class,'notice')]/td[1]/div[2]/div/a");
            }

            if (titleNodes.Count == 0)
            {
                return new Notice[0];
            }

            Notice[] res = titleNodes.Select(x =>
            {
                string innerText = x.InnerText;
                innerText = innerText.Trim();
                innerText = WebUtility.HtmlDecode(innerText);

                string href = x.GetAttributeValue("href", null);
                href = WebUtility.HtmlDecode(href);

                return new Notice("https://cafe.naver.com" + href, innerText);
            }).ToArray();

            return res;
        }

    }
}
