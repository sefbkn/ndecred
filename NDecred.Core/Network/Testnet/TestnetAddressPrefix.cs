namespace NDecred.Core
{
    public class TestnetAddressPrefix : AddressPrefix
    {
        public override string NetworkAddressPrefix => "T";
        public override byte[] PayToPublicKey => new byte[] {0x28, 0xf7};
        public override byte[] PayToPublicKeyHash => new byte[] {0x0f, 0x21};
        public override byte[] PayToScriptHash => new byte[] {0x0e, 0xfc};
        public override byte[] PayToPublicKeyHashEdwards => new byte[] {0x0f, 0x01};
        public override byte[] PayToPublicKeyHashSchnorr => new byte[] {0x0e, 0xe3};
        public override byte[] PrivateKey => new byte[] {0x23, 0x0e};

        public override byte[] HDPrivateKey => new byte[] {0x04, 0x35, 0x83, 0x97};
        public override byte[] HDPublicKey => new byte[] {0x04, 0x35, 0x87, 0xd1};
    }
}