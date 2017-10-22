using System;

namespace GGMREST.Attribute
{
    public enum METHOD
    {
        GET, POST, DELETE, PUT
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class METHODAttribute: System.Attribute
    {
        public METHOD Method { get; private set; }

        public METHODAttribute(METHOD method)
        {
            Method = method;
        }
    }
}