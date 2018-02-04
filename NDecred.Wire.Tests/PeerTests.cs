using System;
using System.Threading;
using System.Threading.Tasks;
using NDecred.Wire;
using Xunit;

namespace NDecred.Network.Tests
{
    public class PeerTests
    {
        [Fact]
        public async Task Connect_AttemptsToInitiateConnectionWorkflow()
        {
            using (var peer = new Peer())
            {
                await peer.ConnectAsync();
                await Task.Delay(-1);                
            }
        }
    }
}