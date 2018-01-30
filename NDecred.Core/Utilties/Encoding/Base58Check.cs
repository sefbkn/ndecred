using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.Core
{
	/// <summary>
	///     Base58Check is a standardized method to serialize and deserialize addresses
	///     in human-readable form.
	///     1. Version information is combined with data (into an 'extended key')
	///     2. The 'extended key' is hashed with BLAKE256
	///     3. The first 4 bytes of the hash array are appended to to the 'extended key'
	///     4. The resulting collection is encoded in base58.
	/// </summary>
	public class Base58Check : Base58
    {
        private const int ChecksumLength = 4;
        private static readonly Func<byte[], byte[]> ChecksumHashAlgorithm = Hash.BLAKE256;

        public string Encode(IEnumerable<byte> versionPrefix, IEnumerable<byte> data, bool isCompressed)
        {
            var suffix = isCompressed ? new byte[] {0x01} : new byte[0];
            var extendedKey = versionPrefix.Concat(data).Concat(suffix).ToArray();
            var checksum = CalculateChecksum(extendedKey);
            var extendedKeyCheck = extendedKey.Concat(checksum).ToArray();

            return base.Encode(extendedKeyCheck);
        }

        public override byte[] Decode(string value)
        {
            var result = base.Decode(value);

            if (!HasValidChecksum(result))
                throw new Base58CheckException("Base58 encoded string does not have a valid checksum");

            return result;
        }

        private static bool HasValidChecksum(byte[] base58Bytes)
        {
            // The last 4 bytes are the checksum
            var payloadLength = base58Bytes.Length - ChecksumLength;
            var extendedKey = base58Bytes.Take(payloadLength).ToArray();
            var extendedKeyChecksum = base58Bytes.Skip(payloadLength).Take(ChecksumLength).ToArray();
            return CalculateChecksum(extendedKey).SequenceEqual(extendedKeyChecksum);
        }

        private static byte[] CalculateChecksum(byte[] extendedKey)
        {
            return ChecksumHashAlgorithm(extendedKey).Take(ChecksumLength).ToArray();
        }
    }
}