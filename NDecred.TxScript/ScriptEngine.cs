using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public class ScriptEngine
    {
        public ScriptStack DataStack { get; }
        public BranchStack BranchStack { get; }

        public ScriptEngine(ScriptStack stack = null)
        {
            BranchStack = new BranchStack();
            DataStack = stack ?? new ScriptStack();
        }

        public void Run(IEnumerable<OpCode> instructions)
        {
            foreach (var opCode in instructions)
            {
                if (opCode.Word == "OP_ENDIF")
                {
                    BranchStack.Pop();
                    continue;
                }

                var branchOp = BranchStack.Peek();
                switch (branchOp)
                {
                    case BranchOp.True:
                        switch (opCode.Word)
                        {
                            case "OP_ELSE":
                                BranchStack.Pop();
                                BranchStack.Push(BranchOp.False);
                                break;
                            default:
                                opCode.Execute(this);
                                continue;
                        }
                        break;
                    case BranchOp.False:
                        switch (opCode.Word)
                        {
                            case "OP_ELSE":
                                BranchStack.Pop();
                                BranchStack.Push(BranchOp.True);
                                break;
                        }
                        break;
                    case BranchOp.Skip:
                        switch (opCode.Word)
                        {
                            case "OP_IF":
                                BranchStack.Push(BranchOp.Skip);
                                break;
                        }
                        break;
                }
            }
        }
    }
}