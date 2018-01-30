namespace NDecred.Core.Blockchain
{
    public struct TxIn
    {
        public TxIn(OutPoint previousOutPoint, uint sequence, long valueIn, uint blockHeight, uint blockIndex,
            TxWitness witness, byte[] signatureScript)
        {
            PreviousOutPoint = previousOutPoint;
            Sequence = sequence;
            ValueIn = valueIn;
            BlockHeight = blockHeight;
            BlockIndex = blockIndex;
            Witness = witness;
            SignatureScript = signatureScript;
        }

        public OutPoint PreviousOutPoint { get; set; }
        public uint Sequence { get; set; }

        public long ValueIn { get; set; }
        public uint BlockHeight { get; set; }
        public uint BlockIndex { get; set; }
        public TxWitness Witness { get; set; }
        public byte[] SignatureScript { get; set; }
    }
}