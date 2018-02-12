using System;
using System.Linq;

namespace NDecred.TxScript
{
    public partial class ScriptEngine
    {
        private void OpNop()
        {
        }
        
        private void OpFalse()
        {
            MainStack.Push(new byte[0]);
        }

        private void OpPushBytes(int length)
        {
            // Read the next length bytes from the execution stack
            var bytes = Script.Bytes
                .Skip(InstructionPointer + 1)
                .Take(length)
                .ToArray();

            if (bytes.Length < length)
                throw new ScriptException("Attempted to read more bytes than available in script");
            if (bytes.Length > length)
                throw new ScriptException($"Somehow read more than {length} bytes.");

            MainStack.Push(bytes);

            // Move the instruction pointer forward to the end of the data.
            // It will be incremented once more, in the Run method.
            InstructionPointer += length;
        }

        private void OpPushData(OpCode opCode)
        {
            var offset = InstructionPointer + 1;
            var scriptBytes = Script.Bytes.Skip(offset);
            
            // The number of bytes to read.
            int takeBytes;

            switch (opCode)
            {
                case OpCode.OP_PUSHDATA1:
                    takeBytes = scriptBytes.First();
                    offset += 1;
                    break;
                case OpCode.OP_PUSHDATA2:
                    takeBytes = BitConverter.ToInt16(Script.Bytes, offset);
                    offset += 2;
                    break;
                case OpCode.OP_PUSHDATA4:
                    takeBytes = BitConverter.ToInt32(Script.Bytes, offset);
                    offset += 4;
                    break;
                default:
                    throw new InvalidOperationException("OpPushData is only valid for OP_PUSHDATA(1|2|4)");
            }

            if(takeBytes < 0)
                throw new ScriptException($"Expected positive integer to succeed opcode {opCode}");
            if(offset + takeBytes > Script.Bytes.Length)
                throw new ScriptException($"Value succeeding {opCode} would read more bytes than available in script");
            
            // Read the next takeBytes bytes from the script and push it on the data stack.
            var bytes = Script.Bytes
                .Skip(offset)
                .Take(takeBytes)
                .ToArray();

            offset += takeBytes;
            InstructionPointer += offset - 1;

            MainStack.Push(bytes);
        }

        // Push a single byte onto the stack
        private void OpPush(byte value)
        {
            MainStack.Push(new[]{value});
        }

        // Push an integer onto the stack.
        // Value is converted to ScriptInteger so consensus rules are followed.
        private void OpPush(int value)
        {
            var scriptInteger = new ScriptInteger(value);
            MainStack.Push(scriptInteger.ToBytes());
        }

        private void OpIf()
        {
            switch (BranchStack.Peek())
            {
                case BranchOption.Skip:
                case BranchOption.False:
                    BranchStack.Push(BranchOption.Skip);
                    break;
                case BranchOption.True:
                    BranchStack.Push(MainStack.PopBool() ? BranchOption.True : BranchOption.False);
                    break;
            }
        }

        private void OpNotIf()
        {
            switch (BranchStack.Peek())
            {
                case BranchOption.Skip:
                case BranchOption.False:
                    BranchStack.Push(BranchOption.Skip);
                    break;
                case BranchOption.True:
                    BranchStack.Push(!MainStack.PopBool() ? BranchOption.True : BranchOption.False);
                    break;
            }
        }

        private void OpElse()
        {
            switch (BranchStack.Peek())
            {
                case BranchOption.Skip:
                    break;
                case BranchOption.False:
                    BranchStack.Replace(BranchOption.True);
                    break;
                case BranchOption.True:
                    BranchStack.Replace(BranchOption.False);
                    break;
            }
        }
        
        private void OpEndIf()
        {
            BranchStack.Discard();
        }

        private void OpVerify()
        {
            if (!MainStack.PopBool())
                throw new VerifyFailedException();
        }
        
        private void OpReturn()
        {
            throw new EarlyReturnException();
        }

        private void OpReserved(OpCode opCode)
        {
            throw new ReservedOpCodeException(opCode);
        }

        private void OpToAltStack()
        {
            AltStack.Push(MainStack.PopBytes());
        }
        
        private void OpFromAltStack()
        {
            MainStack.Push(AltStack.PopBytes());
        }
    }
}