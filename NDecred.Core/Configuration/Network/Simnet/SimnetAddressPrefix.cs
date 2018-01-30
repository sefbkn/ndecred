namespace NDecred.Core
{
    public class SimnetAddressPrefix : AddressPrefix
    {
        public override string NetworkAddressPrefix => "S";
        public override byte[] PayToPublicKey => new byte[] {0x27, 0x6f}; // starts with Sk
        public override byte[] PayToPublicKeyHash => new byte[] {0x0e, 0x91}; // starts with Ss
        public override byte[] PayToScriptHash => new byte[] {0x0e, 0x6c}; // starts with Sc
        public override byte[] PayToPublicKeyHashEdwards => new byte[] {0x0e, 0x71}; // starts with Se
        public override byte[] PayToPublicKeyHashSchnorr => new byte[] {0x0e, 0x53}; // starts with SS
        public override byte[] PrivateKey => new byte[] {0x23, 0x07}; // starts with Ps

        // BIP32 hierarchical deterministic extended key magics
        public override byte[] HDPrivateKey => new byte[] {0x04, 0x20, 0xb9, 0x03}; // starts with sprv
        public override byte[] HDPublicKey => new byte[] {0x04, 0x20, 0xbd, 0x3d}; // starts with spub
    }
}