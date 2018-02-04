using System;
using System.IO;
using System.Linq;
using NDecred.Common;

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
            byte[] signatureScript)
        {
            PreviousOutPoint = previousOutPoint;
            Sequence = sequence;
            ValueIn = valueIn;
            BlockHeight = blockHeight;
            BlockIndex = blockIndex;
            SignatureScript = signatureScript;
        }

        // Non witness data
        public OutPoint PreviousOutPoint { get; }
        public uint Sequence { get; }
        
        // Witness data
        public long ValueIn { get; set; }
        public uint BlockHeight { get; set; }
        public uint BlockIndex { get; set; }
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