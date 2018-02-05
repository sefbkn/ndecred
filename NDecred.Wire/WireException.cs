using System;

namespace NDecred.Wire
{
    public class WireException : Exception
    {
        public WireException(string message = null, Exception innerException = null) : base(message, innerException)
        {
            
        }
    }
}