using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public class OpCode
    {
        public static OpCode OP_0 = new OpCode("OP_FALSE", 0x00, e => e.DataStack.Push(new byte[0]));
        public static OpCode OP_1 = new OpCode("OP_TRUE", 0x01, e => e.DataStack.Push(0x01));
        
        // Flow control
        public static OpCode OP_NOP = new OpCode("OP_NOP", 0x61, e => { });
        public static OpCode OP_IF = new OpCode("OP_IF", 0x63, e => e.BranchStack.Push(e.DataStack.PopBool() ? BranchOp.True : BranchOp.False));
        public static OpCode OP_NOTIF = new OpCode("OP_NOTIF", 0x64, e => e.BranchStack.Push(e.DataStack.PopBool() ? BranchOp.False : BranchOp.True));
        public static OpCode OP_ELSE = new OpCode("OP_ELSE", 0x67, e => throw new InvalidOperationException("OP_ELSE should not be executed"));
        public static OpCode OP_ENDIF = new OpCode("OP_ENDIF", 0x68, e => throw new InvalidOperationException("OP_ENDIF should not be executed"));
        public static OpCode OP_VERIFY = new OpCode("OP_VERIFY", 0x69, e => throw new NotImplementedException("OP_VERIFY"));
        public static OpCode OP_RETURN = new OpCode("OP_RETURN", 0x6a, e => throw new NotImplementedException("OP_RETURN"));

        
        public OpCode(string word, byte code, Action<ScriptEngine> execute)
        {
            Word = word;
            Code = code;
            Execute = execute;
        }
        
        public string Word { get; }
        public byte Code { get; }
        public Action<ScriptEngine> Execute { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is OpCode opCode))
                return false;
            return this.Code == opCode.Code;
        }

        public static bool operator ==(OpCode a, OpCode b)
        {
            return a?.Equals(b) ?? false;
        }

        public static bool operator !=(OpCode a, OpCode b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{Code:X}] {Word}";
        }
    }
}