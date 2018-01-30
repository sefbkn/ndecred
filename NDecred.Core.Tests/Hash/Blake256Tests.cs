using System.Text;
using BlakeSharp;
using Xunit;

namespace NDecred.Core.Tests
{
    public class Blake256Tests
    {
        [Fact]
        public void Blake256_ComputeHash_ReturnsExpectedHash()
        {
            var hashTests = new[]
            {
                new {expected = "716f6e863f744b9ac22c97ec7b76ea5f5908bc5b2f67c61510bfc4751384ea7a", input = ""},
                new {expected = "43234ff894a9c0590d0246cfc574eb781a80958b01d7a2fa1ac73c673ba5e311", input = "a"},
            };

            var hashAlgo = new Blake256(); 
            foreach (var test in hashTests)
            {
                var input = Encoding.UTF8.GetBytes(test.input);
                var hash = hashAlgo.ComputeHash(input);
                var hashHex = Hex.FromByteArray(hash).ToLower();
                Assert.Equal(test.expected, hashHex);
            }
        }
    }
}