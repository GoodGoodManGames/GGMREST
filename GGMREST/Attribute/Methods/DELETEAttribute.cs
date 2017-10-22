using System;

namespace GGMREST.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    [METHOD(METHOD.DELETE)]
    public class DELETEAttribute: ValueAttribute
    {
        public DELETEAttribute(string path)
        {
            Value = path;
        }
    }
}