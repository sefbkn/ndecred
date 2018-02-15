using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public partial class ScriptEngine
    {
        private void OpNop(ParsedOpCode op)
        {
            if (op.Code.IsUpgradableNop() && Script.Options.DiscourageUpgradableNops)
                throw new ReservedOpCodeException(op.Code);
        }

        private void OpDisabled(ParsedOpCode op)
        {
            throw new DisabledOpCodeException(op.Code);
        }
        
        private void OpFalse()
        {
            MainStack.Push(new byte[0]);
        }

        private void OpPushBytes(ParsedOpCode op)
        {
            // Read the next length bytes from the execution stack
            MainStack.Push(op.Data);
        }

        private void OpPushData(ParsedOpCode op)
        {
            MainStack.Push(op.Data);
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

        private void OpReserved(ParsedOpCode op)
        {
            throw new ReservedOpCodeException(op.Code);
        }

        private void OpToAltStack()
        {
            AltStack.Push(MainStack.Pop());
        }
        
        private void OpFromAltStack()
        {
            MainStack.Push(AltStack.Pop());
        }

        private void OpIfDup()
        {
            var bytes = MainStack.Peek();
            
            var scriptInt = new ScriptInteger(bytes, false, bytes.Length);
            if ((int) scriptInt == 0)
                return;
            
            MainStack.Push(bytes);
        }

        private void OpDepth()
        {
            var bytes = new ScriptInteger(MainStack.Count).ToBytes();
            MainStack.Push(bytes);
        }

        private void OpDrop()
        {
            MainStack.Pop();
        }

        private void OpDup()
        {
            MainStack.Push(MainStack[0]);
        }

        private void OpNip()
        {
            MainStack.RemoveAt(1);
        }
        
        private void OpOver()
        {
            MainStack.Push(MainStack[1]);
        }

        private void OpPick()
        {
            var n = MainStack.PopInt32();
            MainStack.Push(MainStack[n]);
        }

        private void OpRoll()
        {
            var n = MainStack.PopInt32();
            var val = MainStack[n];

            MainStack.RemoveAt(n);
            MainStack.Push(val);
        }

        private void OpRot()
        {
            var rotated = new[]
            {
                MainStack[1],
                MainStack[2],
                MainStack[0]
            };
            
            MainStack.RemoveRange(0, 3);
            MainStack.InsertRange(0, rotated);
        }
        
        private void OpSwap()
        {
            var swapped = new[]
            {
                MainStack[1],
                MainStack[0]
            };
            
            MainStack.RemoveRange(0, 2);
            MainStack.InsertRange(0, swapped);
        }

        private void OpTuck()
        {
            MainStack.Insert(2, MainStack[0]);
        }

        private void Op2Drop()
        {
            MainStack.RemoveRange(0, 2);
        }

        private void Op2Dup()
        {
            MainStack.InsertRange(0, MainStack.Take(2));
        }

        private void Op3Dup()
        {
            MainStack.InsertRange(0, MainStack.Take(3));
        }

        private void Op2Over()
        {
            MainStack.InsertRange(0, MainStack.Skip(2).Take(2));
        }

        private void Op2Rot()
        {
            var values = MainStack.Skip(4).Take(2).ToArray();
            MainStack.RemoveRange(4, 2);
            MainStack.InsertRange(0, values);
        }

        private void Op2Swap()
        {
            var top = MainStack.Take(2).ToArray();      
            MainStack.RemoveRange(0, 2);
            MainStack.InsertRange(2, top);
        }

        private void OpCat(ParsedOpCode op)
        {
            var first = MainStack.Pop();
            var second = MainStack.Pop();
            
            if(first.Length + second.Length > Script.Options.MaxScriptElementSize)
                throw new StackElementTooBigException(op.Code);
            
            var bytes = second.Concat(first).ToArray();
            MainStack.Push(bytes);
        }

        private void OpSubStr(ParsedOpCode op)
        {
            var startIndex = MainStack.PopInt32();
            var endIndexExclusive = MainStack.PopInt32();
            var array = MainStack.Pop();
            
            if(array.Length == 0)
                MainStack.Push(new byte[0]);
            else if(startIndex < 0 || endIndexExclusive < 0)
                throw new ScriptException($"{op}: Negative substring index");
            else if(startIndex > array.Length || endIndexExclusive > array.Length)
                throw new ScriptException($"{op}: Substring index out of bounds");
            else if(startIndex > endIndexExclusive)
                throw new ScriptException($"{op}: Start index is greater than end index");
            else if(startIndex == endIndexExclusive)
                MainStack.Push(new byte[0]);
            else
            {
                var substr = array.Skip(startIndex).Take(endIndexExclusive - startIndex).ToArray();
                MainStack.Push(substr);
            }
        }
    }
}