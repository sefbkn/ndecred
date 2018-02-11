using NDecred.Common;

namespace NDecred.TxScript.Tests
{
    public partial class ScriptIntegerTests
    {
        // Test case data from dcrd
        // https://github.com/decred/dcrd/blob/master/txscript/scriptnum_test.go
        // TODO: Add more.
        readonly TestCase[] _serializationTestCases =
        {
            new TestCase(new byte[0], 0, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("01"), 1, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("81"), -1, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("ff00"), 255, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("0001"), 256, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("0081"), -256, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("008000"), 32768, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("008080"), -32768, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("ffffff7f"), 2147483647, ScriptInteger.MathOpcodeMaxLength, true, null),
            new TestCase(Hex.ToByteArray("ffffffff"), -2147483647, ScriptInteger.MathOpcodeMaxLength, true, null),
            
            new TestCase(Hex.ToByteArray("80"), -0, ScriptInteger.MathOpcodeMaxLength, true, typeof(ScriptIntegerEncodingException)) ,
            
            new TestCase(Hex.ToByteArray("810000"), 0, ScriptInteger.MathOpcodeMaxLength, true, typeof(ScriptIntegerEncodingException)),   // 129
            new TestCase(Hex.ToByteArray("810000"), 129, ScriptInteger.MathOpcodeMaxLength, false, null),   // 129
        };
    }
}