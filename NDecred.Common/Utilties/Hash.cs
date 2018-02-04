using System.Security.Cryptography;
using BlakeSharp;
using Org.BouncyCastle.Crypto.Digests;

namespace NDecred.Common
{
    public static class Hash
    {
        public static byte[] SHA256(byte[] data)
        {
            return new SHA256Managed().ComputeHash(data);
        }

        public static byte[] SHA256D(byte[] data)
        {
            return SHA256(SHA256(data));
        }

        public static byte[] RIPEMD160(byte[] data)
        {
            var ripemd = new RipeMD160Digest();
            ripemd.BlockUpdate(data, 0, data.Length);
            var output = new byte[20];
            ripemd.DoFinal(output, 0);
            return output;
        }

        public static byte[] BLAKE256(byte[] data)
        {
            using (var blake256 = new Blake256())
            {
                return blake256.ComputeHash(data);
            }
        }

        public static byte[] BLAKE256D(byte[] data)
        {
            return BLAKE256(BLAKE256(data));
        }
    }
}