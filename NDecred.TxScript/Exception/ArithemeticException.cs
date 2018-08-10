namespace NDecred.TxScript
{
    public class ArithemeticException : ScriptException
    {
        public OpCode OpCode { get; }

        public ArithemeticException(OpCode opCode, string message) : base(message)
        {
            OpCode = opCode;
        }
    }
}