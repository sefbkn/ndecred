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

        public long Value { get; set; }
        public ushort Version { get; set; }
        public byte[] PkScript { get; set; }
    }
}