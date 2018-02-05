using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NDecred.Wire;
using Xunit;

namespace NDecred.Network.Tests
{
    public class PeerTests
    {
        private MemoryStream _writeStream;
        private MemoryStream _readStream;
        private BinaryReader _binaryReader;
        private BinaryWriter _binaryWriter;
        private Mock<INetworkClient> _networkClientMock;
        private CurrencyNet _currencyNet = CurrencyNet.SimNet;
        
        public PeerTests()
        {
            
            _writeStream = new MemoryStream();
            _readStream = new MemoryStream();
            
            _binaryReader = new BinaryReader(_readStream);
            _binaryWriter = new BinaryWriter(_writeStream);
            
            var netClientMock = new Mock<INetworkClient>();
            netClientMock.Setup(m => m.ConnectAsync()).Returns(Task.CompletedTask);
            netClientMock.Setup(m => m.GetStreamWriter()).Returns(_binaryWriter);
            netClientMock.Setup(m => m.IsConnected).Returns(false);
            netClientMock.Setup(m => m.AvailableBytes).Returns((int)_readStream.Length);
            netClientMock.Setup(m => m.GetStreamReader()).Returns(_binaryReader);

            _networkClientMock = netClientMock;
        }
        
        [Fact]
        public async void ConnectAsync_WithMockNetworkClient_ConnectsAndSendsVersionMessage()
        {
            var versionMessage = new MsgVersion(){ProtocolVersion = 1};
            var versionMessageHeader = new MessageHeader(_currencyNet, MsgCommand.Version, versionMessage.Encode());
            var expected = versionMessageHeader.Encode().Concat(versionMessage.Encode()).ToArray();

            var subject = new Peer(_networkClientMock.Object, _currencyNet);
            await subject.ConnectAsync();
            
            _networkClientMock.Verify(m => m.ConnectAsync(), Times.Once);

            // The data written to the stream should be the entire version message
            var actualBytesWritten = _writeStream.ToArray();
            Assert.Equal(expected.Length, actualBytesWritten.Length);
            Assert.Equal(actualBytesWritten, (IEnumerable<byte>)expected);
        }
    }
}