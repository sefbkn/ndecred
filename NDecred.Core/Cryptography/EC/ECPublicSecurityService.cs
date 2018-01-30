using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;

namespace NDecred.Core
{
    public class ECPublicSecurityService : ECSecurityServiceBase
    {
        private readonly byte[] _publicKey;

        public ECPublicSecurityService(byte[] publicKey)
        {
            _publicKey = publicKey;
        }

        public bool VerifySignature(byte[] message, ECSignature signature)
        {
            var ecPoint = CurveParameters.Curve.DecodePoint(_publicKey);
            var publicKeyParameters = new ECPublicKeyParameters("EC", ecPoint, DomainParameters);
            var ecdsaSigner = new ECDsaSigner(new HMacDsaKCalculator(DigestAlgorithm));
            ecdsaSigner.Init(false, publicKeyParameters);
            return ecdsaSigner.VerifySignature(message, signature.R, signature.S);
        }
    }
}