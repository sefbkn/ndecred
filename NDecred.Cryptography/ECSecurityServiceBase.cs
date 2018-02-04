using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;

namespace NDecred.Cryptography
{
    public class ECSecurityServiceBase
    {
        protected IDigest DigestAlgorithm => new Sha256Digest();
        protected X9ECParameters CurveParameters => CustomNamedCurves.GetByOid(SecObjectIdentifiers.SecP256k1);
        protected ECDomainParameters DomainParameters => GetEllipticCurveDomainParameters(CurveParameters);

        protected ECDomainParameters GetEllipticCurveDomainParameters(X9ECParameters parameters)
        {
            return new ECDomainParameters(parameters.Curve, parameters.G, parameters.N, parameters.H);
        }
    }
}