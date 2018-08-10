using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

namespace NDecred.Cryptography
{
    public class ECSecurityServiceBase
    {
        protected IDigest DigestAlgorithm => new Sha256Digest();
        protected X9ECParameters CurveParameters => CustomNamedCurves.GetByOid(SecObjectIdentifiers.SecP256k1);
        protected ECDomainParameters DomainParameters => GetEllipticCurveDomainParameters(CurveParameters);

        public byte[] GetPublicKey(byte[] privateKey, bool isCompressed)
        {
            var privateKeyParameters = GetPrivateKeyParameters(privateKey);
            var publicKeyParameters = GetPublicKeyParameters(DomainParameters, privateKeyParameters);
            var q = publicKeyParameters.Q.Normalize();
            return DomainParameters.Curve
                .CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger())
                .GetEncoded(isCompressed);
        }

        public ECSignature Sign(byte[] privateKey, byte[] data)
        {
            var privateKeyParameters = GetPrivateKeyParameters(privateKey);

            var ecdsaSigner = new ECDsaSigner(new HMacDsaKCalculator(DigestAlgorithm));
            ecdsaSigner.Init(true, privateKeyParameters);
            var signature = ecdsaSigner.GenerateSignature(data);
            return new ECSignature(signature[0], signature[1]);
        }
        
        public bool VerifySignature(byte[] publicKey, byte[] message, ECSignature signature)
        {
            var ecPoint = CurveParameters.Curve.DecodePoint(publicKey);
            var publicKeyParameters = new ECPublicKeyParameters("EC", ecPoint, DomainParameters);
            var ecdsaSigner = new ECDsaSigner(new HMacDsaKCalculator(DigestAlgorithm));
            ecdsaSigner.Init(false, publicKeyParameters);
            
            return ecdsaSigner.VerifySignature(message, signature.R, signature.S);
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

        private ECDomainParameters GetEllipticCurveDomainParameters(X9ECParameters parameters)
        {
            return new ECDomainParameters(parameters.Curve, parameters.G, parameters.N, parameters.H);
        }
    }
}