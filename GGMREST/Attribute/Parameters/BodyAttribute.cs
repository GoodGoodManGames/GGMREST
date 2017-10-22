using System;

namespace GGMREST.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class BodyAttribute: ValueAttribute
    {
    }
}