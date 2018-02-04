using System;
using System.IO;
using System.Linq;
using System.Text;
using NDecred.Common;
using NDecred.Network;
using Xunit;

namespace NDecred.Core.Tests.Wire
{
    public partial class MsgTxTests
    {
	    [Fact]
	    public void New_GivenSerializedValueWithMultipleTx_DeserializesInputAndReserializes()
	    {
		    var tests = new MsgTxTestSubject[]
		    {
			    new MultiTxTestSubject(), 
			    new NoTxTests()
		    };

		    foreach (var test in tests)
		    {
			    var subject = new MsgTx();
			    subject.Decode(test.EncodedMessage);
			    
			    var deserialized = subject.Encode();
			    Assert.True(test.EncodedMessage.SequenceEqual(deserialized));
		    }
	    }

	    [Fact]
	    public void GetHash_GivenTestObjectWithKnownHash_ReturnsExpectedHash()
	    {
		    var expectedHash = "4538fc1618badd058ee88fd020984451024858796be0a1ed111877f887e1bd53";
		    var txIn = new TxIn(
			    previousOutPoint: new OutPoint(
				    hash:  new byte[32],
				    index: 0xffffffff,
				    tree:  TxTree.TxTreeRegular
			    ),
			    sequence:        0xffffffff,
			    valueIn:         5000000000,
			    blockHeight:     0x3F3F3F3F,
			    blockIndex:      0x2E2E2E2E,
			    signatureScript: new byte[]{0x04, 0x31, 0xdc, 0x00, 0x1b, 0x01, 0x62}
		    );

		    var txOut = new TxOut(
			    value: 5000000000,
			    version: 0xf0f0,
			    pkScript: new byte[]
			    {
				    0x41, // OP_DATA_65
				    0x04, 0xd6, 0x4b, 0xdf, 0xd0, 0x9e, 0xb1, 0xc5,
				    0xfe, 0x29, 0x5a, 0xbd, 0xeb, 0x1d, 0xca, 0x42,
				    0x81, 0xbe, 0x98, 0x8e, 0x2d, 0xa0, 0xb6, 0xc1,
				    0xc6, 0xa5, 0x9d, 0xc2, 0x26, 0xc2, 0x86, 0x24,
				    0xe1, 0x81, 0x75, 0xe8, 0x51, 0xc9, 0x6b, 0x97,
				    0x3d, 0x81, 0xb0, 0x1c, 0xc3, 0x1f, 0x04, 0x78,
				    0x34, 0xbc, 0x06, 0xd6, 0xd6, 0xed, 0xf6, 0x20,
				    0xd1, 0x84, 0x24, 0x1a, 0x6a, 0xed, 0x8b, 0x63,
				    0xa6, // 65-byte signature
				    0xac, // OP_CHECKSIG
			    }
		    );
		    
		    var msgTx = new MsgTx {
			    Version = 1, 
			    SerializationType = TxSerializeType.TxSerializeFull,
			    TxIn = new[]{txIn},
			    TxOut = new[]{txOut}
			};

		    var actualHash = msgTx.GetHash().Reverse();
		    var actualHashString = Hex.FromByteArray(actualHash);
		    Assert.Equal(expectedHash, actualHashString);
	    }
	    
	    /// <summary>
	    /// Template to define required values for each test
	    /// </summary>
	    private abstract class MsgTxTestSubject
	    {
		    public abstract byte[] EncodedMessage { get; }
		    public abstract MsgTx Message { get; }
	    }
    }
}
