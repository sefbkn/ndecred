using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public partial class ScriptEngine
    {
        private readonly Dictionary<OpCode, Action<ScriptEngine>> _opCodeLookup;
        public ScriptStack DataStack { get; }
        public BranchStack BranchStack { get; }

        public ScriptEngine(ScriptStack data = null, BranchStack branch = null)
        {
            BranchStack = branch ?? new BranchStack();
            DataStack = data ?? new ScriptStack();
            _opCodeLookup = new Dictionary<OpCode, Action<ScriptEngine>>();
            InitializeOpCodeDictionary();
        }
        
        /// <summary>
        /// Executes a sequence of instructions, taking branching logic into account.
        /// </summary>
        /// <param name="instructions"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Run(OpCode[] instructions)
        {
            foreach (var opCode in instructions)
            {
                var branchOp = BranchStack.Peek();
                var canExecute = branchOp == BranchOption.True || opCode.IsConditional();
                if (!canExecute) continue;
                Execute(opCode);
            }

            if (BranchStack.Count > 1)
            {
                throw new ScriptSyntaxError("Script missing OP_ENDIF");
            }
        }
        
        public void InitializeOpCodeDictionary()
        {
            _opCodeLookup.Add(OpCode.OP_0, OpFalse);
            _opCodeLookup.Add(OpCode.OP_1NEGATE, engine => OpPush(-1));

            // Opcode with values [0x51-0x60]
            // are aliased as OP_1, OP_2, ... OP_N.
            // Each instruction pushes its N value onto the stack.            
            for (byte code = 0x51; code <= 0x60; code++)
            {
                var opCode = (OpCode) code;
                var value = (byte)(code - 0x50);
                _opCodeLookup.Add(opCode, engine => OpPush(value));
            }

            _opCodeLookup.Add(OpCode.OP_NOP, OpNop);
            _opCodeLookup.Add(OpCode.OP_VER, _ => OpReserved(OpCode.OP_VER));
            _opCodeLookup.Add(OpCode.OP_IF, OpIf);
            _opCodeLookup.Add(OpCode.OP_NOTIF, OpNotIf);
            _opCodeLookup.Add(OpCode.OP_ELSE, OpElse);
            _opCodeLookup.Add(OpCode.OP_ENDIF, OpEndIf);
            _opCodeLookup.Add(OpCode.OP_VERIFY, _ => OpVerify());
            

            // Opcodes with values [193, 249]
            // are "unknown", and do nothing.
            for (byte code = 193; code <= 249; code++)
            {
                var opCode = (OpCode) code;
                _opCodeLookup.Add(opCode, OpNop);
            }
        }

        private void Execute(OpCode opCode)
        {
            if (!_opCodeLookup.TryGetValue(opCode, out var action))
                throw new ScriptSyntaxError($"Attempted to execute unknown op code 0x{(byte)opCode:X}");
            
            action(this);
        }
    }
}