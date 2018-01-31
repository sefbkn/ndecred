using System;
using System.IO;
using System.Linq;

namespace NDecred.Core
{
    public class TxIn
    {
        private readonly int _hashCode;
        
        public TxIn(
            uint sequence, 
            long valueIn,
            OutPoint previousOutPoint, 
            uint blockHeight, 
            uint blockIndex,
            TxWitness witness, 
            byte[] signatureScript)
        {
            PreviousOutPoint = previousOutPoint;
            Sequence = sequence;
            ValueIn = valueIn;
            BlockHeight = blockHeight;
            BlockIndex = blockIndex;
            Witness = witness;
            SignatureScript = signatureScript;
        }

        public OutPoint PreviousOutPoint { get; }
        public uint Sequence { get; }
        public long ValueIn { get; set; }
        public uint BlockHeight { get; set; }
        public uint BlockIndex { get; set; }
        public TxWitness Witness { get; set; }
        public byte[] SignatureScript { get; set; }
        

        public void WriteTxInWitnessValueSigning(BinaryWriter writer)
        {
            writer.Write(ValueIn);
            WriteSignatureScript(writer);
        }

        public void WriteSignatureScript(BinaryWriter writer)
        {
            writer.WriteVariableLengthInteger((ulong) SignatureScript.Length);
            writer.Write(SignatureScript);
        }
    }
}