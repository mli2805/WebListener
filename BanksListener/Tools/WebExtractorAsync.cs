using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BanksListener
{
    public class WebExtractorAsync
    {
        public async Task<string> GetPageAsync(string url, string charset, Encoding encoding)
        {
            var request = InitializeHttpWebRequest(url, charset);
            WebResponse response;
            try
            {
                ServicePointManager.SecurityProtocol =  
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                response = await request.GetResponseAsync();
            }
            catch (WebException e)
            {
                OutputException(url, e.Message);
                return "";
            }
            catch (Exception e)
            {
                OutputException(url, e.Message);
                return "";
            }
            return GetWebData(response, encoding);
        }

        public HttpWebRequest InitializeHttpWebRequest(string url, string charset)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = @"text/html; charset=" + charset;
            request.UseDefaultCredentials = true;
            request.UserAgent = "strange";
            request.Timeout = 100000;
            request.ContinueTimeout = 35000;
            request.ReadWriteTimeout = 100000;
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

        private void OutputException(string url, string eMessage)
        {
            Console.WriteLine(url);
            Console.WriteLine(eMessage);
            Console.WriteLine(DateTime.Now);
        }

    }
}
