namespace NDecred.TxScript
{
    public class InvalidOpCodeException : ScriptException
    {
        public OpCode OpCode { get; }

        public InvalidOpCodeException(OpCode opCode) : base("Attempted to call invalid opcode")
        {
            OpCode = opCode;
        }
    }
}