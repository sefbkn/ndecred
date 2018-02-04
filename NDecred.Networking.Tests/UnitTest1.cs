using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using NDecred.Core;
using NDecred.Network;
using Xunit;

namespace NDecred.Networking.Tests
{
    public class UnitTest1
    {
        /*
        [Fact]
        public void Test1()
        {
            var nonce = 12345ul;
            var ping = new Ping(){Nonce = nonce};
            var payload = ping.Encode();
            
            var header = new MessageHeader(CurrencyNet.TestNet2, Command.Ping, payload);
            
            using(var client = new TcpClient("localhost", 19108))
            using(var stream = client.GetStream())
            using(var sr = new BinaryReader(stream))
            using(var sw = new BinaryWriter(stream))
            {
                while (true)
                {
                    // Need to pass streams into constructors.
                    // Then yield values when objects are fully constructed...
                    // don't know how much data will be received to actually pass byte[]
                    // to constructors or when the message is completed.
                    //
                    header.Encode(sw);
                    sw.Write(payload);
                }
            }
        }*/
    }
}