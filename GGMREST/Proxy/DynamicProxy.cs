using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using GGMREST.Attribute;
using Newtonsoft.Json;

namespace GGMREST.Proxy
{
    public class DynamicProxy
    {
        public string BaseUrl { get; set; }
        public IEnumerable<KeyValuePair<string, string>> DefaultHeader { get; set; }

        protected DynamicProxy()
        {
            BaseUrl = "";
            DefaultHeader = new Dictionary<string, string>();
        }

        public bool TryInvokeMember(Type interfaceType, string name, object[] args, out object result)
        {
            //Attribute와 Info들을 저장
            MethodInfo invokeMethodInfo = interfaceType.GetMethod(name);
            ValueAttribute urlAttribute = invokeMethodInfo.GetCustomAttribute<ValueAttribute>();
            METHODAttribute httpMethodAttribute = urlAttribute.GetType().GetCustomAttribute<METHODAttribute>();

            var parameterAttributes = Utility.GetParamterAttributes(invokeMethodInfo, args);
            string apiFormat = urlAttribute.Value;
            string api = Utility.ApiBuilder(apiFormat, parameterAttributes);
            string query = Utility.QueryBuilder(parameterAttributes);
            string bodyJsonString = Utility.RequestBodySerializer(parameterAttributes);

            IRequest request = new Request(string.Concat(BaseUrl, api, query));
            request.Method = httpMethodAttribute.Method.ToString();
            request.SetHeaders(DefaultHeader);
            var headerAttributes = invokeMethodInfo.GetCustomAttributes<HeaderAttribute>();
            foreach (HeaderAttribute headerAttribute in headerAttributes)
                request.SetHeader(headerAttribute.Key, headerAttribute.Value);


            result = Task<object>.Run(() =>
            {
                string responseBodyStr = request.Execute(bodyJsonString);
                Type returnType = invokeMethodInfo.ReturnType.GetGenericArguments()[0];
                return JsonConvert.DeserializeObject(responseBodyStr, returnType);
            });
            return false;
        }

        

    }
}