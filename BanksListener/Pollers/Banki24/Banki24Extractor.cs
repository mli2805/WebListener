using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BanksListener
{
    public class Banki24Extractor
    {
        public async Task<string> GetPageAsync(string url, Encoding encoding)
        {
            var request = InitializeHttpWebRequestForBanki24(url);
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
          
            return GetWebData(response, encoding);
        }

        public HttpWebRequest InitializeHttpWebRequestForBanki24(string url)
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


        private string GetWebData(WebResponse response, Encoding encoding)
        {
            var stream = response.GetResponseStream();
            if (stream == null) return "";
            string webData;
            using (var sr = new StreamReader(stream, encoding))
            {
                webData = sr.ReadToEnd();
            }
            return webData;
        }

    }
}