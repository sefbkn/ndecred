using System;

namespace NDecred.TxScript
{
    public class RuntimeScriptException : ScriptException
    {
        public int InstructionPointer { get; }
        public BranchStack Branch { get; }
        public ScriptStack Data { get; }
        
        public RuntimeScriptException(
            int instructionPointer, 
            ScriptEngine scriptEngine, 
            string message = null,
            Exception innerException = null) : base(message, innerException)
        {
            InstructionPointer = instructionPointer;
            Branch = scriptEngine.BranchStack;
            Data = scriptEngine.DataStack;
        }
    }
}