using System.IO;
using NDecred.Common;

namespace NDecred.Wire
{
    public class TxIn
    {
        public TxIn()
        {
            PreviousOutPoint = new OutPoint();
            SignatureScript = new byte[0];
        }

        // Non witness data
        public OutPoint PreviousOutPoint { get; set; }
        public uint Sequence { get; set; }

        // Witness data
        public long ValueIn { get; set; }
        public uint BlockHeight { get; set; }
        public uint BlockIndex { get; set; }
        public byte[] SignatureScript { get; set; }
    }
}