using System;

namespace NDecred.TxScript
{
    public class RuntimeScriptException : ScriptException
    {
        public int InstructionPointer { get; }
        
        public RuntimeScriptException(
            int instructionPointer, 
            string message = null,
            Exception innerException = null) : base(message, innerException)
        {
            InstructionPointer = instructionPointer;
        }
    }
}