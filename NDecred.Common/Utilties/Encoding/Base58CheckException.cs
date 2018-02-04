using System;

namespace NDecred.Common.Encoding
{
    public class Base58CheckException : Exception
    {
        public Base58CheckException(string message) : base(message)
        {
        }
    }
}