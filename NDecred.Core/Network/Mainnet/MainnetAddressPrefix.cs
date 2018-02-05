

namespace NDecred.Core
{
    public class MainnetAddressPrefix : AddressPrefix
    {
        public override string NetworkAddressPrefix => "D";

        public override byte[] PayToPublicKey => new byte[] {0x13, 0x86};
        public override byte[] PayToPublicKeyHash => new byte[] {0x07, 0x3f};
        public override byte[] PayToScriptHash => new byte[] {0x07, 0x1a};
        public override byte[] PayToPublicKeyHashEdwards => new byte[] {0x07, 0x1f};
        public override byte[] PayToPublicKeyHashSchnorr => new byte[] {0x07, 0x01};
        public override byte[] PrivateKey => new byte[] {0x22, 0xde};

        public override byte[] HDPrivateKey => new byte[] {0x02, 0xfd, 0xa4, 0xe8};
        public override byte[] HDPublicKey => new byte[] {0x02, 0xfd, 0xa9, 0x26};
    }
}