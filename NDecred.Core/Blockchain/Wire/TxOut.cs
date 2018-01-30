namespace NDecred.Core.Blockchain
{
    public struct TxOut
    {
        public TxOut(long value, ushort version, byte[] pkScript)
        {
            Value = value;
            Version = version;
            PkScript = pkScript;
        }

        public long Value { get; }
        public ushort Version { get; }
        public byte[] PkScript { get; }
    }
}