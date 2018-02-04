using NDecred.Wire;
using Xunit;

namespace NDecred.Network.Tests
{
    public class MsgVerAckTests
    {
        [Fact]
        public void DecodeEncode_GivenNoDataToDeserialize_ReturnsNoDataWhenSerialized()
        {
            var subject = new MsgVerAck();
            subject.Decode(new byte[0]);
            var result = subject.Encode();
            
            Assert.True(result != null);
            Assert.Equal(0, result.Length);
        }
    }
}