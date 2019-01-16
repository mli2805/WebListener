using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WebListener.WebExtractorsAsync
{
    public class Banki24Extractor
    {
        public string FetchPage(string page)
        {
            var request = (HttpWebRequest)WebRequest.Create(page);
            request.Headers.Add("Cache-Control", "Max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36";
            request.Host = "banki24.by";
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (Exception)
            {
                return "";
            }
            var stream = response.GetResponseStream();
            if (stream == null) return "";
            string webData;
            using (var sr = new StreamReader(stream))
            {
                webData = sr.ReadToEnd();
            }
            return webData;
        }

        public async Task<string> FetchPageAsync(string page)
        {
            var request = (HttpWebRequest)WebRequest.Create(page);
            request.Headers.Add("Cache-Control", "Max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36";
            request.Host = "banki24.by";
            WebResponse response;
            try
            {
                response = await request.GetResponseAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
            var stream = response.GetResponseStream();
            if (stream == null) return "";
            string webData;
            using (var sr = new StreamReader(stream))
            {
                webData = sr.ReadToEnd();
            }
            return webData;
        }
    }
}