using System;

namespace NDecred.TxScript
{
    public class EmptyScriptStackException : ScriptException
    {        
        public EmptyScriptStackException() : base("Stack empty")
        {
        }
    }
}