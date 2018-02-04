namespace NDecred.Core.Configuration.Network.Simnet
{
    public class Simnet : Network
    {
        public override string Name => "Simnet";
        public override AddressPrefix AddressPrefix => new SimnetAddressPrefix();
    }
}