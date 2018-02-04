namespace NDecred.Wire
{
    public class TxOut
    {
        public TxOut(long value, ushort version, byte[] pkScript)
        {
            Value = value;
            Version = version;
            PkScript = pkScript ?? new byte[0];
        }

        public long Value { get; }
        public ushort Version { get; }
        public byte[] PkScript { get; }
    }
}