using System;
using System.Runtime.Serialization;

namespace NDecred.TxScript
{
    public class ScriptException : Exception
    {
        public ScriptException()
        {    
        }
        
        protected ScriptException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ScriptException(string message) : base(message)
        {
        }

        public ScriptException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}