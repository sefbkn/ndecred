using System;
using System.Runtime.Serialization;

namespace NDecred.TxScript
{   
    public abstract class ScriptException : Exception
    {
        protected ScriptException()
        {
        }

        protected ScriptException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected ScriptException(string message) : base(message)
        {
        }

        protected ScriptException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}