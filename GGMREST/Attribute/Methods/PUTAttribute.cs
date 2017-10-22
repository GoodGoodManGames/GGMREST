using System;

namespace GGMREST.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    [METHOD(METHOD.PUT)]
    public class PUTAttribute: ValueAttribute
    {
        public PUTAttribute(string path)
        {
            Value = path;
        }
    }
}