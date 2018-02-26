using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    /// <summary>
    /// Represents a stack of BranchOp flags.  Used to determine whether
    /// or not a particular branch of code should run.  Allows nested branches
    /// to be executed properly.
    /// </summary>
    public class BranchStack
    {
        private Stack<BranchOption> BranchOpStack { get; }

        public BranchStack()
        {
            BranchOpStack = new Stack<BranchOption>(new[]{BranchOption.True});
        }

        public int Count => BranchOpStack.Count;

        public void Discard()
        {
            if(BranchOpStack.Count == 1)
                throw new ScriptSyntaxErrorException("Unbalanced branch instruction encountered");
            
            BranchOpStack.Pop();
        }

        public BranchOption Peek()
        {
            return BranchOpStack.Peek();
        }

        /// <summary>
        /// Discard the top item on the stack
        /// and replace it with the provided value
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public void Replace(BranchOption op)
        {
            Discard();
            Push(op);
        }

        public void Push(BranchOption op)
        {
            BranchOpStack.Push(op);
        }
    }
}