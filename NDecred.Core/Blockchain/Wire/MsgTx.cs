using System;
using System.IO;

namespace NDecred.Core.Blockchain
{
    public class MsgTx
    {
        private const int MaxMessagePayload = 1024 * 1024 * 32;
        private const int MinTxInPayload = 11 + 32;
        private const int MaxTxInPerMessage = MaxMessagePayload / MinTxInPayload + 1;

        public MsgTx()
        {
        }

        public MsgTx(byte[] rawTx)
        {
            using (var memoryStream = new MemoryStream(rawTx))
            using (var reader = new BinaryReader(memoryStream))
            {
                var version = reader.ReadUInt32();
                Version = (ushort) (version & 0xffff);
                SerializationType = (TxSerializeType) (version >> 16);

                switch (SerializationType)
                {
                    case TxSerializeType.TxSerializeNoWitness:
                        DecodePrefix(reader);
                        break;
                    case TxSerializeType.TxSerializeOnlyWitness:
                        DecodeWitness(reader, false);
                        break;
                    case TxSerializeType.TxSerializeWitnessSigning:
                        DecodeWitnessSigning(reader);
                        break;
                    case TxSerializeType.TxSerializeWitnessValueSigning:
                        DecodeWitnessValueSigning(reader);
                        break;
                    case TxSerializeType.TxSerializeFull:
                        DecodePrefix(reader);
                        DecodeWitness(reader, true);
                        break;
                    default:
                        throw new InvalidDataException($"unrecognized transaction type {SerializationType}");
                }
            }
        }

        public TxSerializeType SerializationType { get; set; }
        public ushort Version { get; set; }
        public TxIn[] TxIn { get; set; }
        public TxOut[] TxOut { get; set; }
        public uint LockTime { get; set; }
        public uint Expiry { get; set; }

        public byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                return memoryStream.ToArray();
            }
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

        private TxIn ReadTxInWitnessValueSigning(BinaryReader reader)
        {
            return new TxIn
            {
                ValueIn = (long) reader.ReadUInt64(),
                SignatureScript = ReadScript(reader)
            };
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

        private TxIn ReadTxInWitnessSigning(BinaryReader reader)
        {
            return new TxIn
            {
                SignatureScript = ReadScript(reader)
            };
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

        private TxIn ReadTxInWitness(BinaryReader reader)
        {
            var valueIn = (long) reader.ReadUInt64();
            var blockHeight = reader.ReadUInt32();
            var blockIndex = reader.ReadUInt32();
            var signatureScript = ReadScript(reader);

            return new TxIn
            {
                ValueIn = valueIn,
                BlockHeight = blockHeight,
                BlockIndex = blockIndex,
                SignatureScript = signatureScript
            };
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

        private TxOut ReadTxOut(BinaryReader reader)
        {
            var value = (long) reader.ReadUInt64();
            var version = reader.ReadUInt16();
            var pkScript = ReadScript(reader);

            return new TxOut(value, version, pkScript);
        }

        private byte[] ReadScript(BinaryReader reader)
        {
            var count = reader.ReadVariableLengthInteger();
            if (count > MaxMessagePayload) throw new Exception("tx script is longer than max allowed length");
            return reader.ReadBytes((int) count);
        }

        private TxIn ReadTxInPrefix(BinaryReader reader, TxSerializeType serializationType)
        {
            if (serializationType == TxSerializeType.TxSerializeOnlyWitness)
                throw new Exception("tried to read a prefix input for a witness only tx");

            var prevOutPoint = ReadOutPoint(reader);
            var sequence = reader.ReadUInt32();

            return new TxIn
            {
                PreviousOutPoint = prevOutPoint,
                Sequence = sequence
            };
        }

        private OutPoint ReadOutPoint(BinaryReader reader)
        {
            var hash = reader.ReadBytes(32);
            var index = reader.ReadUInt32();
            var tree = reader.ReadByte();

            return new OutPoint(hash, index, tree);
        }
    }
}