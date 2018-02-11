using NDecred.Common;

namespace NDecred.TxScript.Tests
{
    public partial class ScriptNumTests
    {
        // Test case data from dcrd
        // 
        readonly TestCase[] _serializationTestCases =
        {
            new TestCase(new byte[0], 0, mathOpCodeMaxScriptNumLen, true, null),
            new TestCase(Hex.ToByteArray("01"), 1, mathOpCodeMaxScriptNumLen, true, null),
            new TestCase(Hex.ToByteArray("81"), -1, mathOpCodeMaxScriptNumLen, true, null),
            new TestCase(Hex.ToByteArray("ff00"), 255, mathOpCodeMaxScriptNumLen, true, null),
            new TestCase(Hex.ToByteArray("0001"), 256, mathOpCodeMaxScriptNumLen, true, null),
            new TestCase(Hex.ToByteArray("0081"), -256, mathOpCodeMaxScriptNumLen, true, null),
            new TestCase(Hex.ToByteArray("008000"), 32768, mathOpCodeMaxScriptNumLen, true, null),
            new TestCase(Hex.ToByteArray("008080"), -32768, mathOpCodeMaxScriptNumLen, true, null),
            new TestCase(Hex.ToByteArray("ffffffff7f"), 1, 10, true, null),
        };

    }
}