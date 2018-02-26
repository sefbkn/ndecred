namespace NDecred.TxScript
{
    public class ScriptIndexOutOfRangeException : ScriptException
    {
        public OpCode OpCode { get; }

        public ScriptIndexOutOfRangeException(OpCode opCode, string message) : base(message)
        {
            OpCode = opCode;
        }
    }
}