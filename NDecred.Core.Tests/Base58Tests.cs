using System.Linq;
using NDecred.Common;
using NDecred.Common.Encoding;
using NDecred.Core.Configuration.Network;
using Xunit;

namespace NDecred.Core.Tests
{
    public class Base58Tests
    {
        private readonly Base58Check _subject = new Base58Check(Hash.BLAKE256D);

        private readonly PublicKeyHash[] _p2pk =
        {
            new PublicKeyHash
            {
                IsValid = true,
                NetworkPrefix = Network.Mainnet.AddressPrefix.PayToPublicKeyHash,
                Address = "DsUZxxoHJSty8DCfwfartwTYbuhmVct7tJu",
                Hash = new byte[]
                {
                    0x27, 0x89, 0xd5, 0x8c, 0xfa, 0x09, 0x57, 0xd2, 0x06, 0xf0,
                    0x25, 0xc2, 0xaf, 0x05, 0x6f, 0xc8, 0xa7, 0x7c, 0xeb, 0xb0
                }
            },

            new PublicKeyHash
            {
                IsValid = true,
                NetworkPrefix = Network.Mainnet.AddressPrefix.PayToPublicKeyHash,
                Address = "DsU7xcg53nxaKLLcAUSKyRndjG78Z2VZnX9",
                Hash = new byte[]
                {
                    0x22, 0x9e, 0xba, 0xc3, 0x0e, 0xfd, 0x6a, 0x69, 0xee, 0xc9,
                    0xc1, 0xa4, 0x8e, 0x04, 0x8b, 0x7c, 0x97, 0x5c, 0x25, 0xf2
                }
            }
        };

        private readonly PublicKeyHash[] _p2sh =
        {
            new PublicKeyHash
            {
                IsValid = true,
                NetworkPrefix = Network.Mainnet.AddressPrefix.PayToScriptHash,
                Address = "DcuQKx8BES9wU7C6Q5VmLBjw436r27hayjS",
                Hash = Hash.RIPEMD160(Hash.BLAKE256(new byte[]
                {
                    0x51, 0x21, 0x03, 0xaa, 0x43, 0xf0, 0xa6, 0xc1, 0x57, 0x30,
                    0xd8, 0x86, 0xcc, 0x1f, 0x03, 0x42, 0x04, 0x6d, 0x20, 0x17,
                    0x54, 0x83, 0xd9, 0x0d, 0x7c, 0xcb, 0x65, 0x7f, 0x90, 0xc4,
                    0x89, 0x11, 0x1d, 0x79, 0x4c, 0x51, 0xae
                }))
            }
        };

        private class PublicKeyHash
        {
            public byte[] NetworkPrefix { get; set; }
            public byte[] Hash { get; set; }
            public string Address { get; set; }
            public bool IsValid { get; set; }
        }

        [Fact]
        public void Base58Check_Encode_AddressPassesValidityCheck()
        {
            foreach (var testCase in _p2pk.Concat(_p2sh))
            {
                var address = _subject.Encode(testCase.NetworkPrefix, testCase.Hash, false);

                if (testCase.IsValid)
                    Assert.Equal(testCase.Address, address);
                else
                    Assert.NotEqual(testCase.Address, address);
            }
        }
    }
}