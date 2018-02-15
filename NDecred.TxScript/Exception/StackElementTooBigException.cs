using System;

namespace NDecred.TxScript
{
    public class StackElementTooBigException : ScriptException
    {
        public StackElementTooBigException(OpCode op) : base($"{op} failed; stack element too big.")
        {
            
        }
    }
}