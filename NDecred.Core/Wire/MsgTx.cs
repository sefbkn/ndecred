using System;
using System.ComponentModel;
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
            return Serialize(this.SerializationType);
        }

        /// <summary>
        /// Serialize the current instance as a byte[] that can be sent over the network.
        /// </summary>
        /// <param name="serializationType">
        /// The serialization method used.
        /// This value will be encoded in the output of this method,
        /// so the SerializationType property of this instance may not match.</param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        public byte[] Serialize(TxSerializeType serializationType)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                var serializedVersion = Version | ((uint)serializationType << 16);
                writer.Write(serializedVersion);
                
                switch (serializationType)
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
                        throw new InvalidEnumArgumentException($"unrecognized serialization type {serializationType}");
                }
   
                return memoryStream.ToArray();
            }            
        }

        /// <summary>
        /// Calculates the BLAKE256 hash of the current instance.  Witness data is not serialized.
        /// </summary>
        /// <returns>The hash as a byte[] with length 32</returns>
        public byte[] GetHash()
        {
            var bytes = Serialize(TxSerializeType.TxSerializeNoWitness);
            return Hash.BLAKE256(bytes);
        }
    }
}