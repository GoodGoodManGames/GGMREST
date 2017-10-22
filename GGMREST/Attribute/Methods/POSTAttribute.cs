using System;

namespace GGMREST.Attribute
{
    [AttributeUsage(AttributeTargets.Method)]
    [METHOD(METHOD.POST)]
    public class POSTAttribute: ValueAttribute
    {
        public POSTAttribute(string path)
        {
            Value = path;
        }
    }
}