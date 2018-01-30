namespace NDecred.Core
{
    public class PrivateKey
    {
        private readonly ECPrivateSecurityService _ecPrivateSecurityService;

        public PrivateKey(byte[] secret)
        {
            _ecPrivateSecurityService = new ECPrivateSecurityService(secret);
            Secret = secret;
        }

        public byte[] Secret { get; }

        public ECSignature Sign(byte[] message)
        {
            return _ecPrivateSecurityService.Sign(message);
        }
    }
}