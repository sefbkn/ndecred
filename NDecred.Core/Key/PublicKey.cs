using NDecred.Common;
using NDecred.Common.Encoding;
using NDecred.Cryptography;

namespace NDecred.Core
{
    public class PublicKey
    {
        private readonly ECPublicSecurityService _ecPublicSecurityService;

        public PublicKey(byte[] publicKeyBytes)
        {
            Bytes = publicKeyBytes;
            _ecPublicSecurityService = new ECPublicSecurityService(publicKeyBytes);
        }

        public byte[] Bytes { get; }

        public string GetPublicAddress(Network network, PublicKeyFormat format)
        {
            var prefix = network.AddressPrefix.PayToPublicKeyHash;
            var isCompressed = format == PublicKeyFormat.PKFCompressed;
            var publicKeyHash = HashUtil.Ripemd160(HashUtil.Blake256(Bytes));
            return new Base58Check(HashUtil.Blake256).Encode(prefix, publicKeyHash, isCompressed);
        }

        public bool VerifySignature(byte[] message, ECSignature signature)
        {
            return _ecPublicSecurityService.VerifySignature(message, signature);
        }
    }
}