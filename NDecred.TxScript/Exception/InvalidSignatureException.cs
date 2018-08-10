namespace NDecred.TxScript
{
    public class InvalidSignatureException : ScriptException
    {
        public InvalidSignatureException(string message) : base(message)
        {
            
        }
    }
}