namespace NDecred.Core.Configuration.Network.Testnet
{
    public class TestNet : Network
    {
        public override string Name => "Testnet";
        public override AddressPrefix AddressPrefix => new TestnetAddressPrefix();
    }
}