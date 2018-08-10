namespace NDecred.TxScript
{
    public enum CheckSequenceError
    {
        NegativeSequence,
        InvalidTxVersion,
        SequenceLockTimeDisabled
    }

    public class CheckSequenceException : ScriptException
    {
        public uint TxInSequence { get; }
        public long StackSequence { get; }
        public CheckSequenceError Error { get; }

        public CheckSequenceException(uint txInSequence, long stackSequence, CheckSequenceError error) :
            base($"CheckLockTimeVerify failed. {error}")
        {
            TxInSequence = txInSequence;
            StackSequence = stackSequence;
            Error = error;
        }
    }
}
