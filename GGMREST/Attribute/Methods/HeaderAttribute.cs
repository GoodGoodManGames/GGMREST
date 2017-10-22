using System;
using System.Collections.Generic;

namespace GGMREST.Attribute
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class HeaderAttribute: System.Attribute
    {
        public string Key { get; }
        public string Value { get; }

        public HeaderAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}