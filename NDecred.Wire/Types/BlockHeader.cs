using System;
using System.IO;
using NDecred.Common;

namespace NDecred.Wire
{
    public class BlockHeader : NetworkEncodable
    {
        // Fixed-size length for a block header when encoded as a byte[]
        public const int BlockHeaderLength = 180;

        public BlockHeader()
        {
            PreviousBlockHash = new byte[0];
            MerkleRoot = new byte[0];
            StakeRoot = new byte[0];
            FinalState = new byte[0];
            ExtraData = new byte[0];
        }

        public int Version { get; set; }
        public byte[] PreviousBlockHash { get; set; }
        public byte[] MerkleRoot { get; set; }
        public byte[] StakeRoot { get; set; }
        public ushort VoteBits { get; set; }
        public byte[] FinalState { get; set; }
        public ushort Voters { get; set; }
        public byte FreshStake { get; set; }
        public byte Revocations { get; set; }
        public uint PoolSize { get; set; }
        public uint Bits { get; set; }
        public long SBits { get; set; }
        public uint Height { get; set; }
        public uint Size { get; set; }
        public DateTime Timestamp { get; set; }
        public uint Nonce { get; set; }
        public byte[] ExtraData { get; set; }
        public uint StakeVersion { get; set; }

        public override void Decode(BinaryReader reader)
        {
            Version = reader.ReadInt32();
            PreviousBlockHash = reader.ReadBytes(32);
            MerkleRoot = reader.ReadBytes(32);
            StakeRoot = reader.ReadBytes(32);
            VoteBits = reader.ReadUInt16();
            FinalState = reader.ReadBytes(6);
            Voters = reader.ReadUInt16();
            FreshStake = reader.ReadByte();
            Revocations = reader.ReadByte();
            PoolSize = reader.ReadUInt32();
            Bits = reader.ReadUInt32();
            SBits = reader.ReadInt64();
            Height = reader.ReadUInt32();
            Size = reader.ReadUInt32();
            Timestamp = DateTimeExtensions.FromUnixTime(reader.ReadUInt32());
            Nonce = reader.ReadUInt32();
            ExtraData = reader.ReadBytes(32);
            StakeVersion = reader.ReadUInt32();
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(PreviousBlockHash);
            writer.Write(MerkleRoot);
            writer.Write(StakeRoot);
            writer.Write(VoteBits);
            writer.Write(FinalState);
            writer.Write(Voters);
            writer.Write(FreshStake);
            writer.Write(Revocations);
            writer.Write(PoolSize);
            writer.Write(Bits);
            writer.Write(SBits);
            writer.Write(Height);
            writer.Write(Size);
            writer.Write((uint) Timestamp.ToUnixTime());
            writer.Write(Nonce);
            writer.Write(ExtraData);
            writer.Write(StakeVersion);
        }

        public byte[] GetHash()
        {
            return HashUtil.Blake256(Encode());
        }
    }
}