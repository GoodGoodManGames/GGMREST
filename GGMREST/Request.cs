using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace GGMREST
{
    public class Request: IRequest
    {
        static List<string> RestrictedHeaders = new List<string>(new string[] {
            "Accept",
            "Connection",
            "Content-Length",
            "Content-Type",
            "Date",
            "Expect",
            "Host",
            "If-Modified-Since",
            "Keep-Alive",
            "Proxy-Connection",
            "Range",
            "Referer",
            "Transfer-Encoding",
            "User-Agent"
        });

        public string Url { get; }
        private readonly HttpWebRequest _request;
        private readonly Type _type = typeof(HttpWebRequest);
        private PropertyInfo[] _requestProperties = typeof(HttpWebRequest).GetProperties();

        public string Method
        {
            get { return _request.Method; }

            set { _request.Method = value; }
        }

        public WebHeaderCollection Headers => _request.Headers;

        public Request(string url)
        {
            Url = url;
            _request = WebRequest.CreateHttp(url);
            _request.Timeout = System.Threading.Timeout.Infinite;
        }

        public void SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var header in headers)
            {
                SetHeader(header.Key, header.Value);
            }
        }

        public void SetHeader(string key, string value)
        {
            string foundHeader = RestrictedHeaders.Find(s => s.ToLower() == key.ToLower());
            if (foundHeader != null)
            {
                PropertyInfo headerPropertyInfo = _type.GetProperty(foundHeader.Replace("-", ""));
                if(headerPropertyInfo.PropertyType == typeof(DateTime))
                    headerPropertyInfo.SetValue(_request, DateTime.Parse(value));
                else if(headerPropertyInfo.PropertyType == typeof(bool))
                    headerPropertyInfo.SetValue(_request, Boolean.Parse(value));
                else if(headerPropertyInfo.PropertyType == typeof(int))
                    headerPropertyInfo.SetValue(_request, int.Parse(value));
                else
                    headerPropertyInfo.SetValue(_request, value);
            }
            else
            {
                Headers.Set(key, value);
            }
        }

        public string Execute(string requestBody = "")
        {
            if (Method.Equals("POST") || Method.Equals("POST"))
            {
                using (var streamWriter = new StreamWriter(_request.GetRequestStream()))
                {
                    //TODO: testCreateJsonStr을 인자인 requesstBody로 교채 하지만 테스트 이후에 할 것.
                    streamWriter.Write(requestBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            string responseBodyStr = "";
            using (StreamReader reader = new StreamReader(_request.GetResponse().GetResponseStream()))
            {
                responseBodyStr = reader.ReadToEnd();
            }

            return responseBodyStr;
        }
    }
}