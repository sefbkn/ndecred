using System;
using System.IO;

namespace NDecred.Core
{
    public partial class MsgTx
    {
        private void EncodePrefix(BinaryWriter writer)
        {
            writer.WriteVariableLengthInteger((ulong) TxIn.Length);
            foreach (var tx in TxIn)
                WriteTxInPrefix(writer, tx);
            
            writer.WriteVariableLengthInteger((ulong) TxOut.Length);
            foreach(var tx in TxOut)
                WriteTxOut(writer, tx);
            
            writer.Write(LockTime);
            writer.Write(Expiry);
        }

        private void WriteTxOut(BinaryWriter writer, TxOut txOut)
        {
            writer.Write((ulong)txOut.Value);
            writer.Write(txOut.Version);
            writer.WriteVariableLengthBytes(txOut.PkScript);
        }
        
        private void WriteTxInPrefix(BinaryWriter writer, TxIn txIn)
        {
            WriteOutPoint(writer, txIn.PreviousOutPoint);
            writer.Write(txIn.Sequence);
        }

        private void WriteOutPoint(BinaryWriter writer, OutPoint outpoint)
        {
            writer.Write(outpoint.Hash);
            writer.Write(outpoint.Index);
            writer.Write(outpoint.Tree);
        }

        private void EncodeWitness(BinaryWriter writer)
        {
            writer.WriteVariableLengthInteger((ulong) TxIn.Length);
            foreach (var tx in TxIn)
                WriteTxInWitness(writer, tx);
        }

        private void WriteTxInWitness(BinaryWriter writer, TxIn tx)
        {
            writer.Write((ulong) tx.ValueIn);
            writer.Write(tx.BlockHeight);
            writer.Write(tx.BlockIndex);
            writer.WriteVariableLengthBytes(tx.SignatureScript);
        }

        private void EncodeWitnessSigning(BinaryWriter writer)
        {
            writer.WriteVariableLengthInteger((ulong) TxIn.Length);
            foreach (var tx in TxIn)
                WriteTxInWitnessSigning(writer, tx);
        }

        private void WriteTxInWitnessSigning(BinaryWriter writer, TxIn tx)
        {
            writer.WriteVariableLengthBytes(tx.SignatureScript);
        }

        private void EncodeWitnessValueSigning(BinaryWriter writer)
        {
            writer.WriteVariableLengthInteger((ulong) TxIn.Length);
            foreach (var tx in TxIn)
                WriteTxInWitnessValueSigning(writer, tx);
        }

        private void WriteTxInWitnessValueSigning(BinaryWriter writer, TxIn tx)
        {
            writer.Write(tx.ValueIn);
            writer.WriteVariableLengthBytes(tx.SignatureScript);
        }
    }
}