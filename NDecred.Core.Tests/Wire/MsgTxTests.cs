using System.IO;
using System.Linq;
using NDecred.Core.Blockchain;
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
			    var subject = new MsgTx(test.EncodedMessage);
			    var deserialized = subject.Serialize();
			    Assert.True(test.EncodedMessage.SequenceEqual(deserialized));
		    }
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
