using NDecred.Wire;
using Xunit;

namespace NDecred.Network.Tests
{
    public class PeerTests
    {
        [Fact]
        public void Connect_AttemptsToInitiateConnectionWorkflow()
        {
            var peer = new Peer();
            peer.Connect();
        }
    }
}