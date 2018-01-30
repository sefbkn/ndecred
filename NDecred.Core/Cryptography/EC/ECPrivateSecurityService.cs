using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace NDecred.Core
{
    public class ECPrivateSecurityService : ECSecurityServiceBase
    {
        public ECPrivateSecurityService(byte[] secret)
        {
            PrivateKeyParameters = GetPrivateKeyParameters(secret);
            PublicKeyParameters = GetPublicKeyParameters(DomainParameters, PrivateKeyParameters);
        }

        private ECPublicKeyParameters PublicKeyParameters { get; }
        private ECPrivateKeyParameters PrivateKeyParameters { get; }

        public byte[] GetPublicKey(bool isCompressed)
        {
            var q = PublicKeyParameters.Q.Normalize();
            return DomainParameters.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger())
                .GetEncoded(isCompressed);
        }

        public ECSignature Sign(byte[] data)
        {
            var ecdsaSigner = new ECDsaSigner(new HMacDsaKCalculator(DigestAlgorithm));
            ecdsaSigner.Init(true, PrivateKeyParameters);
            var signature = ecdsaSigner.GenerateSignature(data);
            return new ECSignature(signature[0], signature[1]);
        }

        private ECPublicKeyParameters GetPublicKeyParameters(ECDomainParameters domainParameters,
            ECPrivateKeyParameters privateKeyParameters)
        {
            var q = domainParameters.G.Multiply(privateKeyParameters.D);
            return new ECPublicKeyParameters(q, DomainParameters);
        }

        private ECPrivateKeyParameters GetPrivateKeyParameters(byte[] secret)
        {
            var secretBytes = new BigInteger(1, secret);
            return new ECPrivateKeyParameters(secretBytes, DomainParameters);
        }
    }
}