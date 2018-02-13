using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    /// <summary>
    /// Represents operations.
    /// </summary>
    public class Script
    {
        public byte[] Bytes { get; }
        public ScriptOptions Options { get; }
        public ParsedOpCode[] ParsedOpCodes { get; }

        public Script(IEnumerable<OpCode> opCodes, ScriptOptions options = null) : 
            this(opCodes.Select(op => (byte) op).ToArray(), options)
        {
        }
        
        public Script(byte[] bytes, ScriptOptions options = null)
        {
            Bytes = bytes;
            Options = options;
            ParsedOpCodes = ParseOpCodes(bytes).ToArray();
        }

        /// <summary>
        /// Parses raw opcode bytes into a collection of
        /// structured op codes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private IEnumerable<ParsedOpCode> ParseOpCodes(byte[] bytes)
        {
            for (int index = 0; index < bytes.Length; index++)
            {
                var opCode = (OpCode) bytes[index];

                // Some opcodes have data encoded in the script that must be parsed.
                // These opcodes should also skip over the bytes consumed by them so
                // each iteration of this loop seeks to the next sequential opcode.
                
                if (opCode.IsOpData())
                    yield return ParseOpData(bytes, ref index);
                else if(opCode.IsPushDataOpCode())
                    yield return ParseOpPushData(bytes, ref index);
                else if (opCode.IsOpN())
                    yield return ParseOpN(bytes, ref index);
                else
                    yield return new ParsedOpCode { Code = opCode, Data = null };
            }
        }

        /// <summary>
        /// Parses the opcodes OP_PUSHDATA{1,2,4}
        /// 
        /// These opcodes are followed by a 'length' parameter and a payload of size 'length'.
        /// </summary>
        /// <exception cref="InvalidOperationException">thrown if an opcode is passed that is not an OP_PUSHDATA</exception>
        /// <exception cref="ScriptException">thrown if the payload length succeeding the opcode is invalid for this script.</exception>
        private ParsedOpCode ParseOpPushData(byte[] bytes, ref int index)
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
                throw new ScriptException($"Expected positive integer to succeed opcode {opCode}");
            if(offset + takeBytes > bytes.Length)
                throw new ScriptException($"Value succeeding {opCode} would read more bytes than available in script");

            // Read the next takeBytes bytes from the script and push it on the data stack.
            var dataBytes = bytes
                .Skip(offset)
                .Take(takeBytes)
                .ToArray();

            offset += takeBytes;
            index += offset - 1;
            
            // Size is the size of the opcode + ALL data mapped to this opcode.
            return new ParsedOpCode {Code = opCode, Data = dataBytes};
        }

        /// <summary>
        /// OP_DATA
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private ParsedOpCode ParseOpData(byte[] bytes, ref int index)
        {
            var opCode = (OpCode) bytes[index];
            var length = OpCode.OP_DATA_1 - opCode + 1;
            var data = bytes.Skip(index + 1).Take(length).ToArray();
            
            if (data.Length < length)
                throw new ScriptException("Attempted to read more bytes than available in script");
            
            // Seek forward
            index += 1 + data.Length;
            
            return new ParsedOpCode
            {
                Code = opCode,
                Data = data
            };
        }

        private ParsedOpCode ParseOpN(byte[] bytes, ref int index)
        {
            return new ParsedOpCode
            {
                Code = (OpCode) bytes[index],
                Data = new[]{(byte) (bytes[index] - 0x50)}
            };
        }
    }
}