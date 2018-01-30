using System;
using System.IO;

namespace NDecred.Core
{
    public partial struct BlockHeader
    {
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
        public uint Timestamp { get; set; }

        // Nonce is techincally a part of ExtraData,
        // but we use it as the classical 4-byte nonce here.
        public uint Nonce { get; set; }

        // ExtraData is used to encode the nonce or any other
        // extra data that might be used later on in consensus
        public byte[] ExtraData { get; set; }

        // Stake version used for voting
        public uint StakeVersion { get; set; }

        // Length is a constant that represents the number of bytes for a block
        // header.
        // blockHeaderLen
        public const int Length = 180;

        // const MaxBlockHeaderPayload = 84 + (chainhash.HashSize * 3)
        // 180 bytes.
        public const int MaxBlockHeaderPayload = 84 * 32 * 3;
    }

    public partial struct BlockHeader
    {
        public BlockHeader(byte[] message)
        {
            if (message.Length != Length)
                throw new Exception($"Invalid block header size {message.Length}");

            FinalState = new byte[6];
            ExtraData = new byte[32];

            using (var ms = new MemoryStream(message))
            using (var br = new BinaryReader(ms))
            {
                Version = br.ReadInt32();
                PreviousBlockHash = br.ReadBytes(32);
                MerkleRoot = br.ReadBytes(32);
                StakeRoot = br.ReadBytes(32);
                VoteBits = br.ReadUInt16();

                var finalState = br.ReadBytes(6);
                Array.Copy(finalState, FinalState, 6);

                Voters = br.ReadUInt16();
                FreshStake = br.ReadByte();
                Revocations = br.ReadByte();
                PoolSize = br.ReadUInt32();
                Bits = br.ReadUInt32();
                SBits = br.ReadInt64();
                Height = br.ReadUInt32();
                Size = br.ReadUInt32();
                Timestamp = br.ReadUInt32();
                Nonce = br.ReadUInt32();

                var extraData = br.ReadBytes(32);
                Array.Copy(extraData, ExtraData, 32);

                StakeVersion = br.ReadUInt32();
            }
        }

        public byte[] Serialize()
        {
            var header = new byte[Length];
            using (var ms = new MemoryStream(header))
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(Version);
                bw.Write(PreviousBlockHash);
                bw.Write(MerkleRoot);
                bw.Write(StakeRoot);
                bw.Write(VoteBits);
                bw.Write(FinalState);
                bw.Write(Voters);
                bw.Write(FreshStake);
                bw.Write(Revocations);
                bw.Write(PoolSize);
                bw.Write(Bits);
                bw.Write(SBits);
                bw.Write(Height);
                bw.Write(Size);
                bw.Write(Timestamp);
                bw.Write(Nonce);
                bw.Write(ExtraData);
                bw.Write(StakeVersion);

                return ms.ToArray();
            }
        }
    }
}