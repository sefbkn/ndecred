namespace NDecred.Core
{
    public class Mainnet : Network
    {
        public override string Name => "Mainnet";
        public override AddressPrefix AddressPrefix => new MainnetAddressPrefix();
    }
}