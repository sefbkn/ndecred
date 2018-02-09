using System;
using System.Collections.Generic;

namespace NDecred.TxScript
{
    /// <summary>
    /// Represents a stack of BranchOp flags.  Used to determine whether
    /// or not a particular branch of code should run.  Allows nested branches
    /// to be executed properly.
    /// </summary>
    public class BranchStack
    {
        private Stack<BranchOp> BranchOpStack { get; }

        public BranchStack()
        {
            BranchOpStack = new Stack<BranchOp>(new[]{BranchOp.True});
        }

        public int Count => BranchOpStack.Count;

        public BranchOp Pop()
        {
            if(BranchOpStack.Count == 1)
                throw new Exception("Syntax error. Unbalanced branch instruction encountered");
            return BranchOpStack.Pop();
        }

        public BranchOp Peek()
        {
            return BranchOpStack.Peek();
        }

        public void Push(BranchOp op)
        {
            BranchOpStack.Push(op);
        }
    }
}