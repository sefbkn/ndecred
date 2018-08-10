namespace NDecred.TxScript
{
    public class ScriptSyntaxErrorException : ScriptException
    {
        public OpCode? OpCode { get; }
        
        public ScriptSyntaxErrorException(OpCode? opCode, string message) : base(message)
        {
            OpCode = opCode;
        }
        
        public ScriptSyntaxErrorException(string message) :this(null, message)
        {
        }
    }
}