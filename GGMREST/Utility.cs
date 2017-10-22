using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GGMREST.Attribute;
using Newtonsoft.Json;

namespace GGMREST
{
    public static class Utility
    {
        public static string ApiBuilder(string apiFormat,
            IEnumerable<KeyValuePair<ValueAttribute, object>> parameterAttributes)
        {
            return ApiBuilder(apiFormat, GetParameterPairs<PathAttribute>(parameterAttributes));
        }

        public static string QueryBuilder(IEnumerable<KeyValuePair<ValueAttribute, object>> parameterAttributes)
        {
            return QueryBuilder(GetParameterPairs<QueryAttribute>(parameterAttributes));
        }

        public static string RequestBodySerializer(
            IEnumerable<KeyValuePair<ValueAttribute, object>> parameterAttributes)
        {
            return RequestBodySerializer(GetParameterPairs<BodyAttribute>(parameterAttributes)
                .Select(pair => pair.Value)
                .ToArray());
        }

        public static IEnumerable<KeyValuePair<ValueAttribute, object>> GetParamterAttributes(
            MethodInfo invokeMethodInfo,
            object[] args)
        {
            return invokeMethodInfo.GetParameters()
                .Where(param => param.GetCustomAttribute<ValueAttribute>() != null)
                .Select(param => new KeyValuePair<ValueAttribute, object>(param.GetCustomAttribute<ValueAttribute>(),
                    args[param.Position]));
        }

        public static KeyValuePair<string, object>[] GetParameterPairs<T>(
            IEnumerable<KeyValuePair<ValueAttribute, object>> parameterAttributes)
        {
            return parameterAttributes
                .Where(paramPair => paramPair.Key is T)
                .Select(paramPair => new KeyValuePair<string, object>(paramPair.Key.Value, paramPair.Value))
                .ToArray();
        }

        public static string ApiBuilder(string apiFormat, KeyValuePair<string, object>[] pathAndParamPairs)
        {
            StringBuilder apiBuilder = new StringBuilder(apiFormat);
            foreach (var pathAndParamPair in pathAndParamPairs)
            {
                apiBuilder.Replace(string.Concat("{", pathAndParamPair.Key, "}"),
                    pathAndParamPair.Value.ToString());
            }
            return apiBuilder.ToString();
        }

        public static string QueryBuilder(KeyValuePair<string, object>[] queryPairs)
        {
            StringBuilder apiBuilder = new StringBuilder("");
            apiBuilder.Append("?");
            foreach (var queryPair in queryPairs)
            {
                apiBuilder.Append(queryPair.Key);
                apiBuilder.Append("=");
                apiBuilder.Append(queryPair.Value);
                apiBuilder.Append("&");
            }
            apiBuilder.Remove(apiBuilder.Length - 1, 1);
            return apiBuilder.ToString();
        }

        public static string RequestBodySerializer(object[] bodies)
        {
            string serializedBodyStr = "";
            if (bodies.Length == 1)
            {
                serializedBodyStr = JsonConvert.SerializeObject(bodies[0]);
            }
            else
            {
                serializedBodyStr = JsonConvert.SerializeObject(bodies);
            }
            return serializedBodyStr;
        }
    }
}