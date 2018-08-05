using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDecred.Common;

namespace NDecred.TxScript
{
    public class Script
    {
        public byte[] Bytes => ParsedOpCodes.SelectMany(p => p.Serialize()).ToArray();
        public ParsedOpCode[] ParsedOpCodes { get; }
        public ParsedOpCode[][] Subscripts { get; }

        public Script(params ParsedOpCode[][] subScripts)
        {
            Subscripts = subScripts;
            ParsedOpCodes = subScripts.SelectMany(s => s).ToArray();
        }

        public Script(IEnumerable<OpCode> opCodes) :
            this(opCodes.Select(op => (byte) op).ToArray()) { }

        public Script(byte[] bytes) :
            this(ParseOpCodes(bytes)) { }

        private static IEnumerable<byte[]> SplitByOpCode(byte[] data, OpCode separator)
        {
            var lastSeparator = 0;
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] != (byte) separator) continue;

                var take = i - lastSeparator;
                if (take == 0) continue;

                yield return data.Skip(lastSeparator).Take(take).ToArray();
                lastSeparator = i + 1;
            }
        }

        public ParsedOpCode[] GetOpCodesWithoutData(params byte[][] values)
        {
            return ParsedOpCodes
                .Where(op => op.IsCanonicalPush())
                .Where(op => !values.Any(val => op.Data.Contains(val))).ToArray();
        }

        /// <summary>
        /// Parses raw opcode bytes into a collection of
        /// structured op codes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static ParsedOpCode[] ParseOpCodes(byte[] bytes)
        {
            var ops = new List<ParsedOpCode>();

            for (var index = 0; index < bytes.Length;)
            {
                var opCode = (OpCode) bytes[index];

                // Some opcodes have data encoded in the script that must be parsed.
                // These opcodes should also skip over the bytes consumed by them so
                // each iteration of this loop seeks to the next sequential opcode.

                if (opCode.IsOpData())
                {
                    ops.Add(ParseOpData(bytes, ref index));
                }
                else if (opCode.IsPushDataOpCode())
                {
                    ops.Add(ParseOpPushData(bytes, ref index));
                }
                else if (opCode.IsOpN())
                {
                    ops.Add(ParseOpN(bytes, index));
                    index++;
                }
                else
                {
                    ops.Add(new ParsedOpCode(opCode));
                    index++;
                }
            }

            return ops.ToArray();
        }

        /// <summary>
        /// Parses the opcodes OP_PUSHDATA{1,2,4}
        ///
        /// These opcodes are followed by a 'length' parameter and a payload of size 'length'.
        /// The payload is extracted from the script and placed in the ParsedOpCode.Data property,
        /// and the position in the script "index" is incremented to skip over the processed bytes.
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown if an opcode is passed that is not an OP_PUSHDATA</exception>
        /// <exception cref="ScriptException">thrown if the payload length succeeding the opcode is invalid for this script.</exception>
        private static ParsedOpCode ParseOpPushData(byte[] bytes, ref int index)
        {
            // The number of bytes to read.
            int takeBytes;
            var offset = index + 1;
            var opCode = (OpCode) bytes[index];

            switch (opCode)
            {
                case OpCode.OP_PUSHDATA1:
                    takeBytes = bytes[offset];
                    offset += 1;
                    break;
                case OpCode.OP_PUSHDATA2:
                    takeBytes = BitConverter.ToInt16(bytes, offset);
                    offset += 2;
                    break;
                case OpCode.OP_PUSHDATA4:
                    takeBytes = BitConverter.ToInt32(bytes, offset);
                    offset += 4;
                    break;
                default:
                    throw new InvalidOperationException("OpPushData is only valid for OP_PUSHDATA(1|2|4)");
            }

            if(takeBytes < 0)
                throw new ScriptSyntaxErrorException(opCode, $"Expected positive integer to succeed opcode {opCode}");
            if(offset + takeBytes > bytes.Length)
                throw new ScriptSyntaxErrorException(opCode, $"Value succeeding {opCode} would read more bytes than available in script");

            // Read the next takeBytes bytes from the script and push it on the data stack.
            var dataBytes = bytes
                .Skip(offset)
                .Take(takeBytes)
                .ToArray();

            offset += takeBytes;
            index = offset;

            // Size is the size of the opcode + ALL data mapped to this opcode.
            return new ParsedOpCode(opCode, dataBytes);
        }

        /// <summary>
        /// OP_DATA
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static ParsedOpCode ParseOpData(byte[] bytes, ref int index)
        {
            var opCode = (OpCode) bytes[index];
            var length = (opCode - OpCode.OP_DATA_1) + 1;
            var data = bytes.Skip(index + 1).Take(length).ToArray();

            if (data.Length < length)
                throw new ScriptSyntaxErrorException(opCode, "Attempted to read more bytes than available in script");

            // Seek forward
            index += 1 + data.Length;

            return new ParsedOpCode(opCode, data);
        }

        private static ParsedOpCode ParseOpN(byte[] bytes, int index)
        {
            var opCode = (OpCode) bytes[index];
            var value = (byte)(opCode - 0x50);
            return new ParsedOpCode((OpCode) bytes[index], new[]{value});
        }

        public override string ToString()
        {
            var tokens = new List<string>();
            foreach (var opCode in ParsedOpCodes)
            {
                tokens.Add(opCode.Code.ToString());
                if (opCode.Data.Length == 0) continue;
                tokens.Add($"0x{Hex.FromByteArray(opCode.Data)}");
            }

            return string.Join(" ", tokens);
        }
    }
}
