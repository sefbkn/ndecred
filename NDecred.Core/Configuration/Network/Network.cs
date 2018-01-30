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
}