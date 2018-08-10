namespace NDecred.TxScript
{
    public class CheckMultisigException : ScriptException
    {
        public CheckMultisigException(string message = null): base(message){}
    }
}
