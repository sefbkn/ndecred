using System.Linq;

namespace NDecred.Core
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

        public override bool Equals(object obj)
        {
            if (!(obj is TxOut txB)) return false;

            return Value == txB.Value
                   && Version == txB.Version
                   && PkScript.SequenceEqual(txB.PkScript);
        }
    }
}