using System;
using System.IO;

namespace NDecred.Core
{
    public partial class MsgTx
    {
        public const int MaxMessagePayload = 1024 * 1024 * 32;
        public const int MinTxInPayload = 11 + 32;
        public const int MaxTxInPerMessage = MaxMessagePayload / MinTxInPayload + 1;


        public MsgTx(
            ushort version,
            TxSerializeType serializeType,
            TxIn[] txIn,
            TxOut[] txOut,
            uint lockTime,
            uint expiry)
        {
            Version = version;
            SerializationType = serializeType;
            TxIn = txIn ?? new TxIn[0];
            TxOut = txOut ?? new TxOut[0];
            LockTime = lockTime;
            Expiry = expiry;
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

        public TxSerializeType SerializationType { get; }
        public ushort Version { get; }
        public TxIn[] TxIn { get; private set; }
        public TxOut[] TxOut { get; private set; }
        public uint LockTime { get; private set; }
        public uint Expiry { get; private set; }
        
        public byte[] Serialize()
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                var serializedVersion = Version | ((uint)SerializationType << 16);
                writer.Write(serializedVersion);
                
                switch (SerializationType)
                {
                    case TxSerializeType.TxSerializeNoWitness:
                        EncodePrefix(writer);
                        break;
                    case TxSerializeType.TxSerializeOnlyWitness:
                        EncodeWitness(writer);
                        break;
                    case TxSerializeType.TxSerializeWitnessSigning:
                        EncodeWitnessSigning(writer);
                        break;
                    case TxSerializeType.TxSerializeWitnessValueSigning:
                        EncodeWitnessValueSigning(writer);
                        break;
                    case TxSerializeType.TxSerializeFull:
                        EncodePrefix(writer);
                        EncodeWitness(writer);
                        break;
                    default:
                        throw new InvalidDataException($"unrecognized transaction type {SerializationType}");
                }

                
                return memoryStream.ToArray();
            }
        }
    }
}