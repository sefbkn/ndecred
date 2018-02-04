using NDecred.Core.Configuration.Network;
using NDecred.Cryptography;
using Xunit;

namespace NDecred.Core.Tests
{
    public class WifTests
    {
        [Fact]
        public void Key_GenerateWif_ReturnsExpectedWifForNetworkAndSignatureType()
        {
            var keys = new[]
            {
                // ECTypeSecp256k1
                new byte[]
                {
                    0x0c, 0x28, 0xfc, 0xa3, 0x86, 0xc7, 0xa2, 0x27,
                    0x60, 0x0b, 0x2f, 0xe5, 0x0b, 0x7c, 0xae, 0x11,
                    0xec, 0x86, 0xd3, 0xbf, 0x1f, 0xbe, 0x47, 0x1b,
                    0xe8, 0x98, 0x27, 0xe1, 0x9d, 0x72, 0xaa, 0x1d
                },
                new byte[]
                {
                    0xdd, 0xa3, 0x5a, 0x14, 0x88, 0xfb, 0x97, 0xb6,
                    0xeb, 0x3f, 0xe6, 0xe9, 0xef, 0x2a, 0x25, 0x81,
                    0x4e, 0x39, 0x6f, 0xb5, 0xdc, 0x29, 0x5f, 0xe9,
                    0x94, 0xb9, 0x67, 0x89, 0xb2, 0x1a, 0x03, 0x98
                },

                // ECTypeEdwards
                new byte[]
                {
                    0x0c, 0x28, 0xfc, 0xa3, 0x86, 0xc7, 0xa2, 0x27,
                    0x60, 0x0b, 0x2f, 0xe5, 0x0b, 0x7c, 0xae, 0x11,
                    0xec, 0x86, 0xd3, 0xbf, 0x1f, 0xbe, 0x47, 0x1b,
                    0xe8, 0x98, 0x27, 0xe1, 0x9d, 0x72, 0xaa, 0x1d
                },
                new byte[]
                {
                    0x0c, 0xa3, 0x5a, 0x14, 0x88, 0xfb, 0x97, 0xb6,
                    0xeb, 0x3f, 0xe6, 0xe9, 0xef, 0x2a, 0x25, 0x81,
                    0x4e, 0x39, 0x6f, 0xb5, 0xdc, 0x29, 0x5f, 0xe9,
                    0x94, 0xb9, 0x67, 0x89, 0xb2, 0x1a, 0x03, 0x98
                },

                // ECTypeSecSchnorr
                new byte[]
                {
                    0x0c, 0x28, 0xfc, 0xa3, 0x86, 0xc7, 0xa2, 0x27,
                    0x60, 0x0b, 0x2f, 0xe5, 0x0b, 0x7c, 0xae, 0x11,
                    0xec, 0x86, 0xd3, 0xbf, 0x1f, 0xbe, 0x47, 0x1b,
                    0xe8, 0x98, 0x27, 0xe1, 0x9d, 0x72, 0xaa, 0x1d
                },
                new byte[]
                {
                    0xdd, 0xa3, 0x5a, 0x14, 0x88, 0xfb, 0x97, 0xb6,
                    0xeb, 0x3f, 0xe6, 0xe9, 0xef, 0x2a, 0x25, 0x81,
                    0x4e, 0x39, 0x6f, 0xb5, 0xdc, 0x29, 0x5f, 0xe9,
                    0x94, 0xb9, 0x67, 0x89, 0xb2, 0x1a, 0x03, 0x98
                }
            };

            var tests = new[]
            {
                // Secp2561k
                new
                {
                    PrivateKey = keys[0],
                    Network = Network.Mainnet,
                    ECDSAType = ECDSAType.ECTypeSecp256k1,
                    Serialized = "PmQdMn8xafwaQouk8ngs1CccRCB1ZmsqQxBaxNR4vhQi5a5QB5716"
                },
                new
                {
                    PrivateKey = keys[1],
                    Network = Network.Testnet,
                    ECDSAType = ECDSAType.ECTypeSecp256k1,
                    Serialized = "PtWVDUidYaiiNT5e2Sfb1Ah4evbaSopZJkkpFBuzkJYcYteugvdFg"
                },
                new
                {
                    PrivateKey = keys[1],
                    Network = Network.Simnet,
                    ECDSAType = ECDSAType.ECTypeSecp256k1,
                    Serialized = "PsURoUb7FMeJQdTYea8pkbUQFBZAsxtfDcfTLGja5sCLZvLZWRtjK"
                },

                // Edwards
                new
                {
                    PrivateKey = keys[2],
                    Network = Network.Mainnet,
                    ECDSAType = ECDSAType.ECTypeEdwards,
                    Serialized = "PmQfJXKC2ho1633ZiVbSdCZw1y68BVXYFpyE2UfDcbQN5xa3DByDn"
                },
                new
                {
                    PrivateKey = keys[3],
                    Network = Network.Testnet,
                    ECDSAType = ECDSAType.ECTypeEdwards,
                    Serialized = "PtWVaBGeCfbFQfgqFew8YvdrSH5TH439K7rvpo3aWnSfDvyK8ijbK"
                },
                new
                {
                    PrivateKey = keys[3],
                    Network = Network.Simnet,
                    ECDSAType = ECDSAType.ECTypeEdwards,
                    Serialized = "PsUSAB97uSWqSr4jsnQNJMRC2Y33iD7FDymZuss9rM6PExexSPyTQ"
                },

                // Schnorr
                new
                {
                    PrivateKey = keys[4],
                    Network = Network.Mainnet,
                    ECDSAType = ECDSAType.ECTypeSecSchnorr,
                    Serialized = "PmQhFGVRUjeRmGBPJCW2FCXFck1EoDBF6hks6auNJVQ26M4h73W9W"
                },
                new
                {
                    PrivateKey = keys[5],
                    Network = Network.Testnet,
                    ECDSAType = ECDSAType.ECTypeSecSchnorr,
                    Serialized = "PtWZ6y56SeRZiuMHBrUkFAbhrURogF7xzWL6PQQJ86XvZfeE3jf1a"
                },
                new
                {
                    PrivateKey = keys[5],
                    Network = Network.Simnet,
                    ECDSAType = ECDSAType.ECTypeSecSchnorr,
                    Serialized = "PsUVgxwa9RM9m5jBoywyzbP3SjPQ7QC4uNEjUVDsTfBeahKkmETvQ"
                }
            };

            foreach (var test in tests)
            {
                // test serialization
                Assert.Equal(test.Serialized, Wif.Serialize(test.Network, test.ECDSAType, false, test.PrivateKey));

                // test deserialization
                Assert.Equal(test.PrivateKey, Wif.Deserialize(test.Network, test.Serialized));
            }
        }
    }
}