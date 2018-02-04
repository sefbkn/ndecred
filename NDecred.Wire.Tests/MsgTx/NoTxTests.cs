using NDecred.Wire;

namespace NDecred.Network.Tests
{
    public partial class MsgTxTests
    {
        private class NoTxTests : MsgTxTests.MsgTxTestSubject
        {
            public override byte[] EncodedMessage => new byte[]
            {
                0x01, 0x00, 0x00, 0x00, // Version
                0x00, // Varint for number of input transactions
                0x00, // Varint for number of output transactions
                0x00, 0x00, 0x00, 0x00, // Lock time
                0x00, 0x00, 0x00, 0x00, // Expiry
                0x00 // Varint for number of input signatures
            };


            public override MsgTx Message => new MsgTx
            {
                Version = 1,
                SerializationType = TxSerializeType.TxSerializeFull
            };
        }
    }
}