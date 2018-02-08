namespace NDecred.Wire
{
    public class TxOut
    {
        public TxOut()
        {
            PkScript = new byte[32];
        }
        
        public TxOut(long value, ushort version, byte[] pkScript)
        {
            Value = value;
            Version = version;
            PkScript = pkScript ?? new byte[0];
        }

        public long Value { get; set; }
        public ushort Version { get; set; }
        public byte[] PkScript { get; set; }
    }
}