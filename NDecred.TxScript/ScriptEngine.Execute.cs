using System;

namespace NDecred.TxScript
{
    public partial class ScriptEngine
    {
        private static void OpNop(ScriptEngine engine)
        {
        }
        
        private static void OpFalse(ScriptEngine engine)
        {
            engine.DataStack.Push(new byte[0]);
        }

        // This is wrong.  Should be pushing a byte[] longer than 1 byte.
        private void OpPushByte(ScriptEngine engine, byte value)
        {
            DataStack.Push(new[]{value});
        }

        private void OpPushInteger(ScriptInteger integer, int maxLength)
        {
            DataStack.Push(integer.ToBytes());
        }

        private void OpIf(ScriptEngine e)
        {
            switch (BranchStack.Peek())
            {
                case BranchOption.Skip:
                case BranchOption.False:
                    e.BranchStack.Push(BranchOption.Skip);
                    break;
                case BranchOption.True:
                    e.BranchStack.Push(e.DataStack.PopBool() ? BranchOption.True : BranchOption.False);
                    break;
            }
        }

        private void OpNotIf(ScriptEngine e)
        {
            switch (BranchStack.Peek())
            {
                case BranchOption.Skip:
                case BranchOption.False:
                    e.BranchStack.Push(BranchOption.Skip);
                    break;
                case BranchOption.True:
                    e.BranchStack.Push(!e.DataStack.PopBool() ? BranchOption.False : BranchOption.True);
                    break;
            }
        }

        private void OpElse(ScriptEngine e)
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
        
        private void OpEndIf(ScriptEngine e)
        {
            BranchStack.Discard();
        }
    }
}