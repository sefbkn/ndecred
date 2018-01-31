using System;
using System.IO;

namespace NDecred.Core
{
    public partial class MsgTx
    {
        private TxIn ReadTxInWitnessValueSigning(BinaryReader reader)
        {
            return new TxIn(
                sequence: 0,
                valueIn: (long) reader.ReadUInt64(),
                previousOutPoint: null,
                blockHeight: 0,
                blockIndex: 0,
                witness: null,
                signatureScript: reader.ReadVariableLengthBytes(MaxMessagePayload)
            );
        }
        
        private TxIn ReadTxInWitnessSigning(BinaryReader reader)
        {
            return new TxIn(
                sequence: 0,
                valueIn: 0,
                previousOutPoint: null,
                blockHeight: 0,
                blockIndex: 0,
                witness: null,
                signatureScript: reader.ReadVariableLengthBytes(MaxMessagePayload)
            );
        }

        private TxIn ReadTxInWitness(BinaryReader reader)
        {
            var valueIn = (long) reader.ReadUInt64();
            var blockHeight = reader.ReadUInt32();
            var blockIndex = reader.ReadUInt32();
            var signatureScript = reader.ReadVariableLengthBytes(MaxMessagePayload);

            return new TxIn(
                sequence: 0,
                valueIn: valueIn,
                previousOutPoint: null,
                blockHeight: blockHeight,
                blockIndex: blockIndex,
                witness: null,
                signatureScript: signatureScript
            );
        }
        
        private TxOut ReadTxOut(BinaryReader reader)
        {
            var value = (long) reader.ReadUInt64();
            var version = reader.ReadUInt16();
            var pkScript = reader.ReadVariableLengthBytes(MaxMessagePayload);

            return new TxOut(value, version, pkScript);
        }
        
        private void DecodeWitnessValueSigning(BinaryReader reader)
        {
            var count = reader.ReadVariableLengthInteger();
            if (count > MaxTxInPerMessage)
                throw new Exception($"transaction count {count} too high to fit into message");

            TxIn = new TxIn[count];
            for (ulong i = 0; i < count; i++)
                TxIn[i] = ReadTxInWitnessValueSigning(reader);
        }


        private void DecodeWitnessSigning(BinaryReader reader)
        {
            var count = reader.ReadVariableLengthInteger();
            if (count > MaxTxInPerMessage)
                throw new Exception($"transaction count {count} too high to fit into message");

            TxIn = new TxIn[count];
            for (ulong i = 0; i < count; i++)
                TxIn[i] = ReadTxInWitnessSigning(reader);
        }

        private void DecodeWitness(BinaryReader reader, bool isFull)
        {
            if (!isFull)
            {
                var count = reader.ReadVariableLengthInteger();
                if (count > MaxTxInPerMessage)
                    throw new Exception($"transaction count {count} too high to fit into message");

                TxIn = new TxIn[count];
                for (ulong i = 0; i < count; i++)
                    TxIn[i] = ReadTxInWitness(reader);
            }
            else
            {
                var count = reader.ReadVariableLengthInteger();
                if (TxIn == null)
                    throw new InvalidOperationException(
                        "TxIn should be parsed before witness data for TxSerializeFull");
                if (count != (ulong) TxIn.Length)
                    throw new Exception(
                        $"transaction count {TxIn.Length} does not equal number of signature scripts {count}");
                if (count > MaxTxInPerMessage)
                    throw new Exception($"transaction count {count} too high to fit into message");

                for (ulong i = 0; i < count; i++)
                {
                    var txInWitness = ReadTxInWitness(reader);

                    TxIn[i].ValueIn = txInWitness.ValueIn;
                    TxIn[i].BlockHeight = txInWitness.BlockHeight;
                    TxIn[i].BlockIndex = txInWitness.BlockIndex;
                    TxIn[i].SignatureScript = txInWitness.SignatureScript;
                }
            }
        }

        private void DecodePrefix(BinaryReader reader)
        {
            var count = reader.ReadVariableLengthInteger();
            TxIn = new TxIn[count];

            for (ulong i = 0; i < count; i++)
                TxIn[i] = ReadTxInPrefix(reader, SerializationType);

            count = reader.ReadVariableLengthInteger();
            TxOut = new TxOut[count];
            for (ulong i = 0; i < count; i++)
                TxOut[i] = ReadTxOut(reader);

            LockTime = reader.ReadUInt32();
            Expiry = reader.ReadUInt32();
        }

        private TxIn ReadTxInPrefix(BinaryReader reader, TxSerializeType serializationType)
        {
            if (serializationType == TxSerializeType.TxSerializeOnlyWitness)
                throw new Exception("tried to read a prefix input for a witness only tx");

            var prevOutPoint = ReadOutPoint(reader);
            var sequence = reader.ReadUInt32();

            return new TxIn(
                sequence: sequence,
                valueIn: 0,
                previousOutPoint: prevOutPoint,
                blockHeight: 0,
                blockIndex: 0,
                witness: null,
                signatureScript: null
            );
        }

        private OutPoint ReadOutPoint(BinaryReader reader)
        {
            var hash = reader.ReadBytes(32);
            var index = reader.ReadUInt32();
            var tree = (TxTree) reader.ReadByte();

            return new OutPoint(hash, index, tree);
        }

    }
}