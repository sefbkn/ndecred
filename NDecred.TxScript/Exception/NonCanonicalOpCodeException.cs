using System.Text;
using Org.BouncyCastle.Utilities.Encoders;

namespace NDecred.TxScript
{
    public class NonCanonicalOpCodeException : ScriptException
    {
        public OpCode OpCode { get; }
        public byte[] Data { get; }

        public NonCanonicalOpCodeException(OpCode opCode, byte[] data) : base($"OpCode not optimal for payload. {opCode} '{Hex.ToHexString(data ?? new byte[0])}'")
        {
            OpCode = opCode;
            Data = data;
        }
    }
}