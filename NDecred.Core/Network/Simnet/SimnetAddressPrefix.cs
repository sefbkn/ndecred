namespace NDecred.Core
{
    public class SimnetAddressPrefix : AddressPrefix
    {
        public override string NetworkAddressPrefix => "S";
        public override byte[] PayToPublicKey => new byte[] {0x27, 0x6f};
        public override byte[] PayToPublicKeyHash => new byte[] {0x0e, 0x91};
        public override byte[] PayToScriptHash => new byte[] {0x0e, 0x6c};
        public override byte[] PayToPublicKeyHashEdwards => new byte[] {0x0e, 0x71};
        public override byte[] PayToPublicKeyHashSchnorr => new byte[] {0x0e, 0x53};
        public override byte[] PrivateKey => new byte[] {0x23, 0x07};

        public override byte[] HDPrivateKey => new byte[] {0x04, 0x20, 0xb9, 0x03};
        public override byte[] HDPublicKey => new byte[] {0x04, 0x20, 0xbd, 0x3d};
    }
}