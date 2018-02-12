namespace NDecred.TxScript
{
    public class EarlyReturnException : ScriptException
    {
        public EarlyReturnException() : base("Encountered return statement in script")
        {
            
        }
    }
}