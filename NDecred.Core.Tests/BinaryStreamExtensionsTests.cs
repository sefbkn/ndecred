using System.IO;
using System.Linq;
using NDecred.Common;
using Xunit;

namespace NDecred.Core.Tests
{
    public class BinaryStreamExtensionsTests
    {
        [Fact]
        public void ReadVariableLengthInteger_GivenInputsAtBoundaries_ReturnsInitialValue()
        {
            // Seed the test with numbers
            // 2^n, 2^n + 1, 2^n -1 where n [0, 63]
            var tests = Enumerable.Range(0, 63)
                .Select(i => (long) 1 << i)
                .SelectMany(i => new[] {i - 1, i, i + 1})
                .ToArray();

            for (var i = 0; i < 63; i++)
            {
                var expected = tests[i];

                using (var ms = new MemoryStream())
                using (var bw = new BinaryWriter(ms))
                using (var br = new BinaryReader(ms))
                {
                    bw.WriteVariableLengthInteger(expected);
                    ms.Position = 0;
                    var value = br.ReadVariableLengthInteger();
                    Assert.Equal(expected, value);
                }
            }
        }

        [Fact]
        public void ReadVariableLengthInteger_GivenInputsAtFlagValues_WritesUShort()
        {
            // Test the flags used to determine the size of the integer.
            // These values should cause a ushort to be written to to the stream.
            var tests = new byte[] {0xFD, 0xFE, 0xFF};

            foreach (var test in tests)
            {
                using (var ms = new MemoryStream())
                using (var bw = new BinaryWriter(ms))
                {
                    bw.WriteVariableLengthInteger(test);

                    // Since data should be encoded as a ushort,
                    // the prefix should be 0xFD
                    var expectedSequence = new byte[] {0xFD, test, 0};
                    var writtenBytes = ms.ToArray();
                    Assert.Equal(expectedSequence, writtenBytes);
                }
            }
        }
    }
}