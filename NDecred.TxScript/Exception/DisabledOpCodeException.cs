namespace NDecred.TxScript
{
    public class DisabledOpCodeException : ScriptException
    {
        public OpCode OpCode { get; }
        
        public DisabledOpCodeException(OpCode opCode) : base($"Attempted to execute disabled op code 0x{opCode:X}")
        {
            OpCode = opCode;
        }
    }
}