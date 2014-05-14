using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace FreelanceManager.Performance
{
    public class CustomWebClient : WebClient
    {
        private string _baseUrl;

        public CustomWebClient(string baseUrl):this()
        {
            _baseUrl = baseUrl;
        }

        public CustomWebClient()
        {
            CookieContainer = new CookieContainer();
        }
        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }

        public string Get(string url)
        {
            return DownloadString(url);
        }

        public string Post(string url, string content)
        {
            var request = GetWebRequest(new Uri(_baseUrl + url));
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(content);

            request.ContentType = "application/json; charset=utf-8";

            request.ContentLength = byteArray.Length;

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public void Authenticate(string email, string password)
        {
            Post("/account/login", JsonConvert.SerializeObject(new { email = email, password = password }));
        }
    }
}
