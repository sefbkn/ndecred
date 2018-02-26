using System;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace NDecred.Cryptography
{
    public class ECSignature
    {
        public ECSignature(BigInteger r, BigInteger s)
        {
            R = r;
            S = s;
        }

        public ECSignature(byte[] signature)
        {
            using (var decoder = new Asn1InputStream(signature))
            {
                var seq = (DerSequence) decoder.ReadObject();
                R = ((DerInteger) seq[0]).Value;
                S = ((DerInteger) seq[1]).Value;
            }
        }

        public BigInteger R { get; }
        public BigInteger S { get; }

        private byte[] Canonicalize(BigInteger value)
        {
            var bytes = new List<byte>(value.ToByteArray());
            if (bytes.Count == 0)
                return new byte[] { 0x00 };
            if((bytes[0] & 0x80) != 0)
                bytes.Insert(0, 0x00);
            return bytes.ToArray();
        }
        
        private void AssertCanonicalPadding(byte[] bytes)
        {
            if(bytes[0] == 0x80)
                throw new Exception("Negative signature value encountered");
            if(bytes.Length > 1 && bytes[0] == 0x00 && (bytes[1] & 0x80) != 0x80)
                throw new Exception("Excessive padding in signature value");
        }
        
        public byte[] Serialize()
        {
            // Usually 70-72 bytes.
            using (var ms = new MemoryStream(72))
            {
                var seq = new DerSequenceGenerator(ms);
                seq.AddObject(new DerInteger(R));
                seq.AddObject(new DerInteger(S));
                seq.Close();
                return ms.ToArray();
            }
        }
    }
}