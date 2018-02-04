using Xunit;

namespace NDecred.Core.Tests
{
    public class UInt256Tests
    {
        [Fact]
        public void UInt256_BooleanOpEq_AssertsVal2DoesNotEqualsVal1()
        {
            var val1 = new uint256("EF00000000000000000000000000000000000000000000000000000000000001");
            var val2 = new uint256("EF00000000000000000000000000000000000000000000000000000000000000");
            Assert.True(val2 != val1);
        }

        [Fact]
        public void UInt256_BooleanOpEq_AssertsVal2EqualsVal1()
        {
            var val1 = new uint256("EF00000000000000000000000000000000000000000000000000000000000000");
            var val2 = new uint256("EF00000000000000000000000000000000000000000000000000000000000000");
            Assert.True(val2 == val1);
        }

        [Fact]
        public void UInt256_BooleanOpGt_AssertsVal2IsGreatherThanVal1()
        {
            var val1 = new uint256("0000000000000000000000000000000000000000000000000000000000000001");
            var val2 = new uint256("1000000000000000000000000000000000000000000000000000000000000000");
            Assert.True(val2 > val1);
        }

        [Fact]
        public void UInt256_BooleanOpLt_AssertsVal2IsGreatherThanVal1()
        {
            var val1 = new uint256("0000000000000000000000000000000000000000000000000000000000000001");
            var val2 = new uint256("1000000000000000000000000000000000000000000000000000000000000000");
            Assert.True(val1 < val2);
        }

        [Fact]
        public void UInt256_GetByteAtPosition_ReturnsLeastSignificantByte()
        {
            var expected = (byte) 0xEF;
            var value = new uint256("00000000000000000000000000000000000000000000000000000000000000EF");
            Assert.Equal(expected, value[0]);
        }

        [Fact]
        public void UInt256_GetByteAtPosition_ReturnsMostSignificantByte()
        {
            var expected = (byte) 0xAA;
            var value = new uint256("AA000000000000000000000000000000000000000000000000000000000000EF");
            Assert.Equal(expected, value[31]);
        }

        [Fact]
        public void UInt256_GetBytes_ReturnsBytesInBigEndian()
        {
            var expected = new byte[] {0x50, 0x30, 0x10, 0xEF};
            var value = new uint256("AA000000000000000000000000000000000000000000000000000000503010EF");
            Assert.Equal(expected, value[0, 4]);
        }

        [Fact]
        public void UInt256_New_ConstructsExpectedValueFromHexString()
        {
            var expectedValue = new uint256(new byte[32]
            {
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 1
            });

            var actualValue = new uint256("0000000000000000000000000000000000000000000000000000000000000001");
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void UInt256_SetByte_SetsCorrectByte()
        {
            var expected = new uint256("AA0000000000000000000000000000000000000000000000000000005030AAEF");
            var actual = new uint256("AA000000000000000000000000000000000000000000000000000000503010EF");
            actual[1] = 0xAA;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UInt256_ToHex_ReturnsExpectedValue()
        {
            var value = new uint256(new byte[32]
            {
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 1
            });
            var hexValue = value.ToHex();
            Assert.Equal("0000000000000000000000000000000000000000000000000000000000000001", hexValue);
        }
    }
}