namespace NDecred.TxScript
{
    public enum CheckLockTimeFailureReason
    {
        MismatchedLockTimeTypes,
        NegativeLockTime,
        ScriptLockTimeGreaterThanTxLockTime,
        TxInputFinalized
    }

    public class CheckLockTimeException : ScriptException
    {
        public uint TxLockTime { get; }
        public long ScriptLockTime { get; }
        public CheckLockTimeFailureReason Reason { get; }

        public CheckLockTimeException(uint txLockTime, long scriptLockTime, CheckLockTimeFailureReason reason) :
            base($"CheckLockTimeVerify failed. {reason}")
        {
            TxLockTime = txLockTime;
            ScriptLockTime = scriptLockTime;
            Reason = reason;
        }
    }
}
