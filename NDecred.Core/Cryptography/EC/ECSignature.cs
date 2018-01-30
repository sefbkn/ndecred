using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace NDecred.Core
{
    public class ECSignature
    {
        public ECSignature(BigInteger r, BigInteger s)
        {
            R = r;
            S = s;
        }

        public ECSignature(byte[] derSignature)
        {
            using (var decoder = new Asn1InputStream(derSignature))
            {
                var seq = (DerSequence) decoder.ReadObject();
                R = ((DerInteger) seq[0]).Value;
                S = ((DerInteger) seq[1]).Value;
            }
        }

        public BigInteger R { get; }
        public BigInteger S { get; }

        public byte[] ToDer()
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