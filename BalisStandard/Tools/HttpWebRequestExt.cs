using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BalisStandard
{
    public static class HttpWebRequestExt
    {
        public static async Task<string> GetDataAsync(this HttpWebRequest request)
        {
            try
            {
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var response = await request.GetResponseAsync();
                return GetWebData(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }

        }

        public static HttpWebRequest InitializeForKombanks(this HttpWebRequest request)
        {
            request.ContentType = @"text/html; charset=utf-8";
            request.UseDefaultCredentials = true;
            request.UserAgent = "strange";
            request.Timeout = 100000;
            request.ContinueTimeout = 35000;
            request.ReadWriteTimeout = 100000;
            return request;
        }

        public static HttpWebRequest InitializeForBanki24(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("Cache-Control", "Max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.80 Safari/537.36";
            request.Host = "banki24.by";
            return request;
        }

        public static HttpWebRequest InitializeToGetArchive(this HttpWebRequest request)
        {
            request.Accept = "*/*";
            
            return request;
        }

        private static string GetWebData(WebResponse response)
        {
            var stream = response.GetResponseStream();
            if (stream == null) return "";
            string webData;
            using (var sr = new StreamReader(stream, Encoding.UTF8))
            {
                webData = sr.ReadToEnd();
            }
            return webData;
        }
    }
}