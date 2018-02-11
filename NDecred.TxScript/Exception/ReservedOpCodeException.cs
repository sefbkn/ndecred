namespace NDecred.TxScript
{
    public class ReservedOpCodeException : ScriptException
    {
        public OpCode OpCode { get; }
        
        public ReservedOpCodeException(OpCode opCode) : base($"Attempted to execute reserved op code 0x{opCode:X}")
        {
            OpCode = opCode;
        }
    }
}