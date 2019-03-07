using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebListener
{
    public class WebExtractor
    {
        public async Task<string> GetPageAsync(string url, string charset, Encoding encoding)
        {
            var request = InitializeHttpWebRequest(url, charset);
            request.UseDefaultCredentials = true;
            request.UserAgent = "strange";
            WebResponse response;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | 
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
            return GetWebData(encoding, response);
        }

        private string GetWebData(Encoding encoding, WebResponse response)
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

        private HttpWebRequest InitializeHttpWebRequest(string url, string charset)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ContentType = @"text/html; charset=" + charset;
            request.Timeout = 100000;
            request.ContinueTimeout = 35000;
            request.ReadWriteTimeout = 100000;
            return request;
        }

        private void OutputException(string url, string eMessage)
        {
            Console.WriteLine(url);
            Console.WriteLine(eMessage);
            Console.WriteLine(DateTime.Now);
        }

    }
}
