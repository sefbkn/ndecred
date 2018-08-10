using System;

namespace NDecred.TxScript
{
    public class ScriptIntegerEncodingException : ScriptException
    {
        public byte[] ErrorBytes { get; }
        
        public ScriptIntegerEncodingException(
            byte[] errorBytes,
            string message = null, 
            Exception innerException = null) : base(message, innerException)
        {
            ErrorBytes = errorBytes;
        }
    }
}