namespace NDecred.Core.Configuration.Network.Mainnet
{
    public class MainnetAddressPrefix : AddressPrefix
    {
        public override string NetworkAddressPrefix => "D";

        public override byte[] PayToPublicKey => new byte[] {0x13, 0x86}; // starts with Dk
        public override byte[] PayToPublicKeyHash => new byte[] {0x07, 0x3f}; // starts with Ds
        public override byte[] PayToScriptHash => new byte[] {0x07, 0x1a}; // starts with Dc
        public override byte[] PayToPublicKeyHashEdwards => new byte[] {0x07, 0x1f}; // starts with De
        public override byte[] PayToPublicKeyHashSchnorr => new byte[] {0x07, 0x01}; // starts with DS
        public override byte[] PrivateKey => new byte[] {0x22, 0xde}; // starts with Pm

        // BIP32 hierarchical deterministic extended key magics
        public override byte[] HDPrivateKey => new byte[] {0x02, 0xfd, 0xa4, 0xe8}; // starts with dprv
        public override byte[] HDPublicKey => new byte[] {0x02, 0xfd, 0xa9, 0x26}; // starts with dpub
    }
}