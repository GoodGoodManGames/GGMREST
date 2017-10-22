using System;

namespace GGMREST.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    [METHOD(METHOD.GET)]
    public class GETAttribute: ValueAttribute
    {
        public GETAttribute(string path)
        {
            Value = path;
        }
    }
}