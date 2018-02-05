using System.ComponentModel;
using System.IO;
using NDecred.Common;

namespace NDecred.Wire
{
    public partial class MsgTx : Message
    {
        public const int MaxMessagePayload = 1024 * 1024 * 32;
        public const int MinTxInPayload = 11 + 32;
        public const int MaxTxInPerMessage = MaxMessagePayload / MinTxInPayload + 1;

        public MsgTx()
        {
            TxIn = new TxIn[0];
            TxOut = new TxOut[0];
        }

        public TxSerializeType SerializationType { get; set; }
        public ushort Version { get; set; }
        public TxIn[] TxIn { get; set; }
        public TxOut[] TxOut { get; set; }
        public uint LockTime { get; set; }
        public uint Expiry { get; set; }

        public override void Decode(BinaryReader reader)
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

        public override void Encode(BinaryWriter writer)
        {
            Encode(writer, SerializationType);
        }

        /// <summary>
        ///     Serialize the current instance as a byte[] that can be sent over the network.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="serializationType">
        ///     The serialization method used.
        ///     This value will be encoded in the output of this method,
        ///     so the SerializationType property of this instance may not match.
        /// </param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private void Encode(BinaryWriter writer, TxSerializeType serializationType)
        {
            var serializedVersion = Version | ((uint) serializationType << 16);
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
        }

        /// <summary>
        ///     Calculates the BLAKE256 hash of the current instance.  Witness data is not serialized.
        /// </summary>
        /// <returns>The hash as a byte[] with length 32</returns>
        public byte[] GetHash()
        {
            byte[] bytes;

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                Encode(bw, TxSerializeType.TxSerializeNoWitness);
                bw.Flush();
                bytes = ms.ToArray();
            }

            return Hash.BLAKE256(bytes);
        }

        public override MsgCommand Command => MsgCommand.Tx;
    }
}