using System.Text;
using Org.BouncyCastle.Utilities.Encoders;

namespace NDecred.TxScript
{
    public class NonCanonicalOpCodeException : ScriptException
    {
        public OpCode OpCode { get; }
        public byte[] Data { get; }

        public NonCanonicalOpCodeException(OpCode opCode, byte[] data) : base(BuildMessage(opCode, data))
        {
            OpCode = opCode;
            Data = data;
        }

        private static string BuildMessage(OpCode opCode, byte[] data)
        {
            var msgData = Hex.ToHexString(data ?? new byte[0]);
            var canonicalOpcode = OpCodeUtil.CanonicalOpCodeForData(data);
            return "OpCode not optimal for payload. " +
                   $"Encountered {opCode} rather than " +
                   $"{canonicalOpcode} for '{msgData}'";
        }
    }
}