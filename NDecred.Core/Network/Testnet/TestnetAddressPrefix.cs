namespace NDecred.Core
{
    public class TestnetAddressPrefix : AddressPrefix
    {
        public override string NetworkAddressPrefix => "T";
        public override byte[] PayToPublicKey => new byte[] {0x28, 0xf7}; // starts with Tk
        public override byte[] PayToPublicKeyHash => new byte[] {0x0f, 0x21}; // starts with Ts
        public override byte[] PayToScriptHash => new byte[] {0x0e, 0xfc}; // starts with Tc
        public override byte[] PayToPublicKeyHashEdwards => new byte[] {0x0f, 0x01}; // starts with Te
        public override byte[] PayToPublicKeyHashSchnorr => new byte[] {0x0e, 0xe3}; // starts with TS
        public override byte[] PrivateKey => new byte[] {0x23, 0x0e}; // starts with Pt

        // BIP32 hierarchical deterministic extended key magics
        public override byte[] HDPrivateKey => new byte[] {0x04, 0x35, 0x83, 0x97}; // starts with sprv
        public override byte[] HDPublicKey => new byte[] {0x04, 0x35, 0x87, 0xd1}; // starts with spub
    }
}