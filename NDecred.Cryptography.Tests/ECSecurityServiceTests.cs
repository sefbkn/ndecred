using System.Text;
using NDecred.Common;
using Xunit;

namespace NDecred.Cryptography.Tests
{
    public class ECSecurityServiceTests
    {
        // Uses test data from dcrd
        // https://github.com/decred/dcrd/blob/master/dcrec/secp256k1/example_test.go
        private readonly byte[] _message =
            Encoding.UTF8.GetBytes("test message");

        private readonly byte[] _signature =
            Hex.ToByteArray(
                "3045022100fcc0a8768cfbcefcf2cadd7cfb0fb18ed08dd2e2ae84bef1a474a3d351b26f0302200fc1a350b45f46fa00101391302818d748c2b22615511a3ffd5bb638bd777207");

        private readonly byte[] _privateKey =
            Hex.ToByteArray("22a47fa09a223f2aa079edf85a7c2d4f8720ee63e502ee2869afab7de234b80c");

        [Fact]
        public void ECSecurityService_VerifySignature_VerifiesGeneratedSignatureAgainstPublicKey()
        {
            const bool isCompressed = false;
            var messageHash = Hash.BLAKE256(_message);
            var signature = new ECSignature(_signature);

            var privateSecurityService = new ECPrivateSecurityService(_privateKey);
            var publicKeyBytes = privateSecurityService.GetPublicKey(isCompressed);
            var publicSecurityService = new ECPublicSecurityService(publicKeyBytes);

            var hasValidSignature = publicSecurityService.VerifySignature(messageHash, signature);

            Assert.True(hasValidSignature);
        }

        [Fact]
        public void PrivateKey_Sign_ReturnsExpectedSignatureDer()
        {
            var privateKey = new ECPrivateSecurityService(_privateKey);
            var messageHash = Hash.BLAKE256(_message);
            var signature = privateKey.Sign(messageHash).ToDer();

            Assert.Equal(_signature, signature);
        }
    }
}