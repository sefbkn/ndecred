using System.Collections.Generic;

namespace NDecred.Core.Configuration.Network
{
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
}