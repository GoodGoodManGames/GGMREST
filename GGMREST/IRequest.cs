using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace GGMREST
{
    public interface IRequest
    {
        string Method { get; set; }
        void SetHeaders(IEnumerable<KeyValuePair<string, string>> headers);
        void SetHeader(string key, string value);
        string Execute(string requestBody = "");
    }
}
