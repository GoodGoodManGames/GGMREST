using System;

namespace GGMREST.Proxy.Exception
{
    public class WrongReturnTypeException : System.Exception
    {
        public WrongReturnTypeException(Type retrunType) : base($"Method's return type is not allowed, return type is must take Task<T> form.") { }
    }
}