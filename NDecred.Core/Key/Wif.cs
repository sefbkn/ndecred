using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDecred.Common;
using NDecred.Common.Encoding;
using NDecred.Cryptography;

namespace NDecred.Core
{
    /// <summary>
    ///     Provides methods for encoding / decoding a private key in the Wif format.
    /// </summary>
    public static class Wif
    {
        // 2 bytes for the network info + 1 byte for the ecdsa signature type
        private const int PrefixLength = 3;

        // Length of a private key, in bytes
        private const int KeyLength = 32;

        private static readonly Base58Check Base58Check = new Base58Check(Hash.BLAKE256);

        public static string Serialize(Network network, ECDSAType type, bool isCompressed, byte[] privateKey)
        {
            if (privateKey.Length != KeyLength)
                throw new ArgumentException(nameof(privateKey),
                    $"Private key should be {KeyLength} bytes, not {privateKey.Length}");

            var prefix = BuildPrefix(network, type);
            return Base58Check.Encode(prefix, privateKey, isCompressed);
        }

        public static byte[] Deserialize(Network network, string wif)
        {
            var extendedKeyCheck = Base58Check.Decode(wif);

            var prefix = ExtractPrefix(extendedKeyCheck);
            var isValidWifForNetwork = network.AddressPrefix.All.Any(p => p.SequenceEqual(prefix.networkPrefix));
            if (!isValidWifForNetwork)
                throw new InvalidDataException(
                    $"Wif is not valid for network {network.Name}.  Unrecognized prefix {Hex.FromByteArray(prefix.networkPrefix)}");

            return extendedKeyCheck.Skip(PrefixLength).Take(KeyLength).ToArray();
        }

        private static (byte[] networkPrefix, ECDSAType signatureType) ExtractPrefix(byte[] base58Decoded)
        {
            var prefixBytes = base58Decoded.Take(PrefixLength).ToArray();
            var networkPrefix = prefixBytes.Take(2).ToArray();
            var signatureType = (ECDSAType) prefixBytes[2];
            return (networkPrefix, signatureType);
        }

        private static IEnumerable<byte> BuildPrefix(Network network, ECDSAType type)
        {
            return network.AddressPrefix.PrivateKey.Concat(new[] {(byte) type});
        }
    }
}