using System.Linq;
using NDecred.Blockchain;
using NDecred.Common;
using NDecred.Wire;
using Xunit;

namespace NDecred.Network.Tests
{
    public partial class MsgTxTests
    {
        // Uses test data from dcrd
        // https://github.com/decred/dcrd/blob/master/wire/msgtx_test.go
        string expectedHash = "4538fc1618badd058ee88fd020984451024858796be0a1ed111877f887e1bd53";

        TxIn txIn = new TxIn
        {
            PreviousOutPoint = new OutPoint
            {
                Hash = new byte[32],
                Index = 0xffffffff,
                Tree = (byte) TxTree.TxTreeRegular
            },
            Sequence = 0xffffffff,
            ValueIn = 5000000000,
            BlockHeight = 0x3F3F3F3F,
            BlockIndex = 0x2E2E2E2E,
            SignatureScript = new byte[] {0x04, 0x31, 0xdc, 0x00, 0x1b, 0x01, 0x62}
        };

        TxOut txOut = new TxOut(
            5000000000,
            0xf0f0,
            new byte[]
            {
                0x41, // OP_DATA_65
                0x04, 0xd6, 0x4b, 0xdf, 0xd0, 0x9e, 0xb1, 0xc5,
                0xfe, 0x29, 0x5a, 0xbd, 0xeb, 0x1d, 0xca, 0x42,
                0x81, 0xbe, 0x98, 0x8e, 0x2d, 0xa0, 0xb6, 0xc1,
                0xc6, 0xa5, 0x9d, 0xc2, 0x26, 0xc2, 0x86, 0x24,
                0xe1, 0x81, 0x75, 0xe8, 0x51, 0xc9, 0x6b, 0x97,
                0x3d, 0x81, 0xb0, 0x1c, 0xc3, 0x1f, 0x04, 0x78,
                0x34, 0xbc, 0x06, 0xd6, 0xd6, 0xed, 0xf6, 0x20,
                0xd1, 0x84, 0x24, 0x1a, 0x6a, 0xed, 0x8b, 0x63,
                0xa6, // 65-byte signature
                0xac // OP_CHECKSIG
            }
        );


        /// <summary>
        ///     Template to define required values for each test
        /// </summary>
        private abstract class MsgTxTestSubject
        {
            public abstract byte[] EncodedMessage { get; }
            public abstract MsgTx Message { get; }
        }

        [Fact]
        public void GetHash_GivenTestObjectWithKnownHash_ReturnsExpectedHash()
        {
            var msgTx = new MsgTx
            {
                Version = 1,
                SerializationType = TxSerializeType.Full,
                TxIn = new[] {txIn},
                TxOut = new[] {txOut}
            };

            var actualHash = msgTx.GetHash().Reverse();
            var actualHashString = Hex.FromByteArray(actualHash);
            Assert.Equal(expectedHash, actualHashString);
        }

        [Fact]
        public void New_GivenSerializedValueWithMultipleTx_DeserializesInputAndReserializes()
        {
            var tests = new MsgTxTestSubject[]
            {
                new MultiTxTestSubject(),
                new NoTxTests()
            };

            foreach (var test in tests)
            {
                var subject = new MsgTx();
                subject.Decode(test.EncodedMessage);

                var deserialized = subject.Encode();
                Assert.True(test.EncodedMessage.SequenceEqual(deserialized));
            }
        }
/*
        [Fact]
        public void GetRoot_GivenTestnetGenesisTranasction_ReturnsExpectedMerkleRoot()
        {
            var testnetGenesisTx = new MsgTx
            {
                SerializationType = TxSerializeType.Full,
                Version = 1,
                TxIn = new []
                {
                    new TxIn
                    {
                        PreviousOutPoint = new OutPoint
                        {
                            Hash = new byte[32],
                            Index = 0xffffffff,
                        },
                        SignatureScript = new byte[]
                        {
                            0x04, 0xff, 0xff, 0x00, 0x1d, 0x01, 0x04, 0x45, 
                            0x54, 0x68, 0x65, 0x20, 0x54, 0x69, 0x6d, 0x65, 
                            0x73, 0x20, 0x30, 0x33, 0x2f, 0x4a, 0x61, 0x6e, 
                            0x2f, 0x32, 0x30, 0x30, 0x39, 0x20, 0x43, 0x68, 
                            0x61, 0x6e, 0x63, 0x65, 0x6c, 0x6c, 0x6f, 0x72, 
                            0x20, 0x6f, 0x6e, 0x20, 0x62, 0x72, 0x69, 0x6e, 
                            0x6b, 0x20, 0x6f, 0x66, 0x20, 0x73, 0x65, 0x63, 
                            0x6f, 0x6e, 0x64, 0x20, 0x62, 0x61, 0x69, 0x6c, 
                            0x6f, 0x75, 0x74, 0x20, 0x66, 0x6f, 0x72, 0x20, 
                            0x62, 0x61, 0x6e, 0x6b, 0x73,
                        },
                        Sequence = 0xffffffff
                    },
                },
                TxOut = new TxOut[]
                {
                    new TxOut()
                    {
                        Value = 0x00000000,
                        PkScript = new byte[]
                        {
                            0x41, 0x04, 0x67, 0x8a, 0xfd, 0xb0, 0xfe, 0x55,
                            0x48, 0x27, 0x19, 0x67, 0xf1, 0xa6, 0x71, 0x30,
                            0xb7, 0x10, 0x5c, 0xd6, 0xa8, 0x28, 0xe0, 0x39,
                            0x09, 0xa6, 0x79, 0x62, 0xe0, 0xea, 0x1f, 0x61,
                            0xde, 0xb6, 0x49, 0xf6, 0xbc, 0x3f, 0x4c, 0xef,
                            0x38, 0xc4, 0xf3, 0x55, 0x04, 0xe5, 0x1e, 0xc1,
                            0x12, 0xde, 0x5c, 0x38, 0x4d, 0xf7, 0xba, 0x0b,
                            0x8d, 0x57, 0x8a, 0x4c, 0x70, 0x2b, 0x6b, 0xf1,
                            0x1d, 0x5f, 0xac
                        },
                    },
                },
                LockTime = 0,
                Expiry = 0,
            };

            // For some reason, the testnet genesis tx is hashed differently than mainnet to
            // get the merkle root.
            var hash = testnetGenesisTx.GetHash();
            var merkleTree = new MerkleTree();
            var merkleRoot = merkleTree.GetRoot(new[] {hash});
        }
*/
    }
}