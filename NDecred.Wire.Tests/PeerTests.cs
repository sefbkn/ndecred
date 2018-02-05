using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NDecred.Common;
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
            
            _binaryWriter = new BinaryWriter(_writeStream);
            _binaryReader = new BinaryReader(_readStream);
            
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

        [Fact]
        public async void New_AfterConnecting_FiresMessageReceivedWhenDataAvailable()
        {
            var versionMessage = new MsgVersion(){ProtocolVersion = 1};
            var versionMessageHeader = new MessageHeader(_currencyNet, MsgCommand.Version, versionMessage.Encode());
            
            // Write the header + message to the READ stream, then reset the stream position.
            var writer = new BinaryWriter(_readStream);
            versionMessageHeader.Encode(writer);
            versionMessage.Encode(writer);
            _readStream.Position = 0;
            
            var subject = new Peer(_networkClientMock.Object, _currencyNet);
            await subject.ConnectAsync();
            
            // Closure to capture the data passed to the raised event
            PeerMessageReceivedArgs eventArgs = null;            
            var resetEvent = new ManualResetEvent(false);
            subject.MessageReceived += (sender, e) =>
            {
                eventArgs = e;
                resetEvent.Set();
            };
            
            resetEvent.WaitOne(TimeSpan.FromSeconds(1));
            
            Assert.True(eventArgs != null);
            Assert.Equal(versionMessageHeader.Checksum, (IEnumerable<byte>)eventArgs.Header.Checksum);
        }
    }
}