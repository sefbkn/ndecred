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

        private void OpPush(byte value)
        {
            DataStack.Push(new[]{value});
        }

        private void OpPush(int value)
        {
            var scriptInteger = new ScriptInteger(value);
            DataStack.Push(scriptInteger.ToBytes());
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
                    e.BranchStack.Push(!e.DataStack.PopBool() ? BranchOption.True : BranchOption.False);
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