using System;

namespace NDecred.Core
{
    public class Base58CheckException : Exception
    {
        public Base58CheckException(string message) : base(message)
        {
        }
    }
}