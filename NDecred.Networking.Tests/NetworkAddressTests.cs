using System;
using System.Linq;
using NDecred.Common;
using Xunit;

namespace NDecred.Network.Tests
{
    public class NetworkAddressTests
    {
        private readonly byte[] baseNetAddrEncoded =
        {
            0x29, 0xab, 0x5f, 0x49, // Timestamp
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // SFNodeNetwork
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xff, 0xff, 0x7f, 0x00, 0x00, 0x01, // IP 127.0.0.1
            0x20, 0x8d // Port 8333 in big-endian
        };

        [Fact]
        public void EncodeDecode_ReturnsInputDataExactly()
        {
            var netAddr = new NetworkAddress();
            netAddr.Decode(baseNetAddrEncoded);

            var unixTs = BitConverter.ToInt32(new byte[] {0x29, 0xab, 0x5f, 0x49}, 0);
            var timestamp = DateTimeExtensions.FromUnixTime(unixTs);

            Assert.Equal(timestamp.Date, new DateTime(2009, 1, 3));
            Assert.Equal(timestamp, netAddr.Timestamp);

            Assert.Equal(netAddr.Services, ServiceFlag.SFNodeNetwork);
            Assert.Equal(netAddr.Port, 8333);

            var bytes = netAddr.Encode();
            Assert.True(bytes.SequenceEqual(baseNetAddrEncoded));
        }
    }
}