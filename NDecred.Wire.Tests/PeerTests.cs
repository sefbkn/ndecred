using System;
using System.Net;
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
            ServicePointManager.DefaultConnectionLimit = 200;
            
            using (var server = new Server())
            {
                var ip = new IPAddress(new byte[] {127, 0, 0, 1});
                var endpoint = new IPEndPoint(ip, 19108);
                
                var peer = await server.ConnectToPeerAsync(endpoint);
                await peer.SendMessageAsync(new MsgGetAddr());
                await Task.Delay(-1);                
            }
        }
    }
}