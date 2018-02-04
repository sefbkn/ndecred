using NDecred.Core.Configuration.Network.Testnet;

namespace NDecred.Core.Configuration.Network
{
    public abstract class Network
    {
        public static Network Mainnet => new Mainnet.Mainnet();
        public static Network Testnet => new TestNet();
        public static Network Simnet => new Simnet.Simnet();

        public abstract string Name { get; }
        public abstract AddressPrefix AddressPrefix { get; }
    }
}