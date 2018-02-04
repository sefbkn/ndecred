namespace NDecred.Core.Configuration.Network.Mainnet
{
    public class Mainnet : Network
    {
        public override string Name => "Mainnet";
        public override AddressPrefix AddressPrefix => new MainnetAddressPrefix();
    }
}