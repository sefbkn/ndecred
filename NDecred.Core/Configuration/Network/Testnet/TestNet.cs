namespace NDecred.Core
{
    public class TestNet : Network
    {
        public override string Name => "Testnet";
        public override AddressPrefix AddressPrefix => new TestnetAddressPrefix();
    }
}