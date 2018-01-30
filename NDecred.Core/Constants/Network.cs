using System.Collections.Generic;

namespace NDecred.Core
{
    public abstract class Network
    {
        public static Network Mainnet => new Mainnet();
        public static Network Testnet => new TestNet();
        public static Network Simnet => new Simnet();

        public abstract string Name { get; }
        public abstract AddressPrefix AddressPrefix { get; }
    }

    public class Mainnet : Network
    {
        public override string Name => "Mainnet";
        public override AddressPrefix AddressPrefix => new MainnetAddressPrefix();
    }

    public class TestNet : Network
    {
        public override string Name => "Testnet";
        public override AddressPrefix AddressPrefix => new TestnetAddressPrefix();
    }

    public class Simnet : Network
    {
        public override string Name => "Simnet";
        public override AddressPrefix AddressPrefix => new SimnetAddressPrefix();
    }

    public abstract class AddressPrefix
    {
        public IEnumerable<byte[]> All => new[]
        {
            PayToPublicKey, PayToPublicKeyHash, PayToScriptHash,
            PayToPublicKeyHashEdwards, PayToPublicKeyHashSchnorr,
            PrivateKey, HDPrivateKey, HDPublicKey
        };

        public abstract string NetworkAddressPrefix { get; }
        public abstract byte[] PayToPublicKey { get; }
        public abstract byte[] PayToPublicKeyHash { get; }
        public abstract byte[] PayToScriptHash { get; }
        public abstract byte[] PayToPublicKeyHashEdwards { get; }
        public abstract byte[] PayToPublicKeyHashSchnorr { get; }
        public abstract byte[] PrivateKey { get; }
        public abstract byte[] HDPublicKey { get; }
        public abstract byte[] HDPrivateKey { get; }
    }

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