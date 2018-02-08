using System.Linq;
using System.Security.Cryptography;
using BlakeSharp;
using Org.BouncyCastle.Crypto.Digests;

namespace NDecred.Common
{
    public static class HashUtil
    {
        public static byte[] Sha256(byte[] data)
        {
            return new SHA256Managed().ComputeHash(data);
        }

        public static byte[] Sha256D(byte[] data)
        {
            return Sha256(Sha256(data));
        }

        public static byte[] Ripemd160(byte[] data)
        {
            var ripemd = new RipeMD160Digest();
            ripemd.BlockUpdate(data, 0, data.Length);
            var output = new byte[20];
            ripemd.DoFinal(output, 0);
            return output;
        }

        public static byte[] Blake256(params byte[][] data)
        {
            var combined = data.SelectMany(arr => arr);
            return Blake256(combined.ToArray());
        }

        public static byte[] Blake256(byte[] data)
        {
            using (var blake256 = new Blake256())
            {
                return blake256.ComputeHash(data);
            }
        }

        public static byte[] Blake256D(byte[] data)
        {
            return Blake256(Blake256(data));
        }
    }
}