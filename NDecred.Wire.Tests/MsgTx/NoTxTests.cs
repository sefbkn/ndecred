﻿using NDecred.Wire;

namespace NDecred.Network.Tests
{
    public partial class MsgTxTests
    {
        private class NoTxTests : MsgTxTests.MsgTxTestSubject
        {
            // Uses test data from dcrd
            // https://github.com/decred/dcrd/blob/master/wire/msgtx_test.go
            public override byte[] EncodedMessage => new byte[]
            {
                // Copyright (c) 2013-2016 The btcsuite developers
                // Copyright (c) 2015-2017 The Decred developers

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
                SerializationType = TxSerializeType.Full
            };
        }
    }
}