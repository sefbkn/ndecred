using System;
using System.IO;
using NDecred.Common;

namespace NDecred.Network
{
    public class MsgBlockHeader : NetworkMessage
    {
        // const MaxBlockHeaderPayload = 84 + (chainhash.HashSize * 3)
        // 180 bytes.
        public const int MaxBlockHeaderPayload = 84 + 32 * 3;

        // Length is a constant that represents the number of bytes for a block
        // header.
        // blockHeaderLen
        public const int Length = 180;

        public MsgBlockHeader()
        {
            PreviousBlockHash = new byte[0];
            MerkleRoot = new byte[0];
            StakeRoot = new byte[0];
            FinalState = new byte[0];
            ExtraData = new byte[0];
        }

        // Version of the block.  This is not the same as the protocol version.
        public int Version { get; set; }

        // Hash of the previous block in the block chain.
        public byte[] PreviousBlockHash { get; set; }

        // Merkle tree reference to hash of all transactions for the block.
        public byte[] MerkleRoot { get; set; }

        // Merkle tree reference to hash of all stake transactions for the block.
        public byte[] StakeRoot { get; set; }

        // Votes on the previous merkleroot and yet undecided parameters
        public ushort VoteBits { get; set; }

        // Final state of the PRNG used for ticket selection in the lottery
        public byte[] FinalState { get; set; }

        // Number of participating voters for this block.
        public ushort Voters { get; set; }

        // Number of new sstx in this block
        public byte FreshStake { get; set; }

        // Number of ssrtx present in this block
        public byte Revocations { get; set; }

        // Size of the ticket pool
        public uint PoolSize { get; set; }

        // Difficulty target for the block
        public uint Bits { get; set; }

        // Stake difficulty target
        public long SBits { get; set; }

        // Height is the block height in the block chain
        public uint Height { get; set; }

        // Size is the size of the serialized block in its entirety
        public uint Size { get; set; }

        // Time the block was created.  This is, unfortunately, encoded as a
        // uint32 on the wire and therefore is limited to 2106.
        public DateTime Timestamp { get; set; }

        // Nonce is techincally a part of ExtraData,
        // but we use it as the classical 4-byte nonce here.
        public uint Nonce { get; set; }

        // ExtraData is used to encode the nonce or any other
        // extra data that might be used later on in consensus
        public byte[] ExtraData { get; set; }

        // Stake version used for voting
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
            writer.Write((uint)Timestamp.ToUnixTime());
            writer.Write(Nonce);
            writer.Write(ExtraData);
            writer.Write(StakeVersion);
        }
        
        public byte[] GetHash()
        {
            return Hash.BLAKE256(Encode());
        }
    }
}
