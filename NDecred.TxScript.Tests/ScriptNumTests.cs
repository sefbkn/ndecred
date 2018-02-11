using System;
using System.Collections.Generic;
using NDecred.Common;
using Xunit;

namespace NDecred.TxScript.Tests
{
    public partial class ScriptNumTests
    {
        public const int mathOpCodeMaxScriptNumLen = 4;
        
        [Fact]
        public void ToBytes_ReturnsExpectedValue()
        {

            foreach (var test in _serializationTestCases)
            {
                try
                {   
                    var subject = new ScriptInteger(test.Serialized, test.NumLen);
                    Assert.Equal(test.Serialized, (IEnumerable<byte>)subject.ToBytes());
                }
                catch (Exception e)
                {
                    Assert.Equal(test.GetType(), e.GetType());
                }
            }
        }

        private class TestCase
        {
            public byte[] Serialized { get; }
            public long Number { get; }
            public int NumLen { get; }
            public bool MinimalEncoding { get; }
            public Exception ExpectedException { get; }

            public TestCase(byte[] serialized, long number, int numLen, bool minimalEncoding, Exception expectedException)
            {
                Serialized = serialized;
                Number = number;
                NumLen = numLen;
                MinimalEncoding = minimalEncoding;
                ExpectedException = expectedException;
            }
        }
    }
}