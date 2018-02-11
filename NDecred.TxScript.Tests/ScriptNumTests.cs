using System;
using System.Collections.Generic;
using NDecred.Common;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace NDecred.TxScript.Tests
{
    public partial class ScriptIntegerTests
    {   
        [Fact]
        public void ToBytes_ReturnsExpectedValue()
        {

            foreach (var test in _serializationTestCases)
            {
                try
                {
                    var subject = new ScriptInteger(test.Serialized, test.MinimalEncoding, test.NumLen);
                    
                    // Test deserialization
                    Assert.Equal(test.Number, (int) subject);
                    
                    // Assert serialization works with minimal encoding.
                    if(test.MinimalEncoding)
                        Assert.Equal(test.Serialized, subject.ToBytes());
                    
                    if(test.ExpectedException != null)
                        throw new Exception($"Expected exception of type {test.ExpectedException} not thrown");
                }
                catch (ScriptException e)
                {                    
                    Assert.Equal(test.ExpectedException, e.GetType());
                }
            }
        }

        private class TestCase
        {
            public byte[] Serialized { get; }
            public long Number { get; }
            public int NumLen { get; }
            public bool MinimalEncoding { get; }
            public Type ExpectedException { get; }

            public TestCase(byte[] serialized, long number, int numLen, bool minimalEncoding, Type expectedException)
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