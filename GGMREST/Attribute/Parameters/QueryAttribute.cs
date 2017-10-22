using System;

namespace GGMREST.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class QueryAttribute : ValueAttribute
    {

        public QueryAttribute(string value)
        {
            Value = value;
        }

    }
}