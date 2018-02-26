namespace NDecred.TxScript
{
    public class ReservedOpCodeException : ScriptException
    {
        public OpCode OpCode { get; }
        
        public ReservedOpCodeException(OpCode opCode) : base("Attempted to call reserved opcode")
        {
            OpCode = opCode;
        }
    }
}