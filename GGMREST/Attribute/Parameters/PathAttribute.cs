using System;

namespace GGMREST.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PathAttribute : ValueAttribute
    {
        public PathAttribute(string value)
        {
            Value = value;
        }
    }
}