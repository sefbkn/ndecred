using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public partial class ScriptEngine
    {
        private readonly Dictionary<OpCode, Action> _opCodeLookup;
        
        public int InstructionPointer { get; private set; }
        
        public Script Script { get; }
        
        public ScriptStack AltStack { get; }
        public ScriptStack MainStack { get; }
        public BranchStack BranchStack { get; }

        public ScriptEngine(Script script, ScriptStack mainStack = null, BranchStack branchStack = null)
        {
            Script = script;
            MainStack = mainStack ?? new ScriptStack();
            BranchStack = branchStack ?? new BranchStack();
            AltStack = new ScriptStack();
            _opCodeLookup = new Dictionary<OpCode, Action>();

            InitializeOpCodeDictionary();
        }
        
        /// <summary>
        /// Executes a sequence of instructions, taking branching logic into account.
        /// </summary>
        public void Run()
        {
            while (InstructionPointer < Script.Bytes.Length)
            {
                var opCode = (OpCode) Script.Bytes[InstructionPointer];
                var branchOp = BranchStack.Peek();
                var canExecute = branchOp == BranchOption.True || opCode.IsConditional();
                if (!canExecute) continue;
                Execute(opCode);
                InstructionPointer++;
            }

            if (BranchStack.Count > 1)
            {
                throw new ScriptSyntaxError("Script missing OP_ENDIF");
            }
        }
        
        public void InitializeOpCodeDictionary()
        {
            var collection = new (OpCode op, Action action)[]
            {
                (OpCode.OP_0, OpFalse),
                (OpCode.OP_1NEGATE, () => OpPush(-1)),
                (OpCode.OP_VER, () => OpReserved(OpCode.OP_VER)),
                (OpCode.OP_VERIF, () => OpReserved(OpCode.OP_VERIF)),
                (OpCode.OP_VERNOTIF, () => OpReserved(OpCode.OP_VERNOTIF)),
                (OpCode.OP_NOP, OpNop),
                (OpCode.OP_IF, OpIf),
                (OpCode.OP_NOTIF, OpNotIf),
                (OpCode.OP_ELSE, OpElse),
                (OpCode.OP_ENDIF, OpEndIf),
                (OpCode.OP_VERIFY, OpVerify),
                (OpCode.OP_RETURN, OpReturn),
                (OpCode.OP_PUSHDATA1, () => OpPushData(OpCode.OP_PUSHDATA1)),
                (OpCode.OP_PUSHDATA2, () => OpPushData(OpCode.OP_PUSHDATA2)),
                (OpCode.OP_PUSHDATA4, () => OpPushData(OpCode.OP_PUSHDATA4)),
                (OpCode.OP_RESERVED, () => OpReserved(OpCode.OP_RESERVED)),
                
                (OpCode.OP_TOALTSTACK, OpToAltStack),
                (OpCode.OP_FROMALTSTACK, OpFromAltStack),
                (OpCode.OP_IFDUP, OpIfDup),
                (OpCode.OP_DEPTH, OpDepth),
                (OpCode.OP_DROP, OpDrop),
                (OpCode.OP_DUP, OpDup),
                (OpCode.OP_NIP, OpNip),
                (OpCode.OP_OVER, OpOver),
                (OpCode.OP_PICK, OpPick),
                (OpCode.OP_ROLL, OpRoll),
                (OpCode.OP_ROT, OpRot),
                (OpCode.OP_SWAP, OpSwap),
                (OpCode.OP_TUCK, OpTuck),
                (OpCode.OP_2DROP, Op2Drop),
                (OpCode.OP_2DUP, Op2Dup),
                (OpCode.OP_3DUP, Op3Dup),

                (OpCode.OP_2OVER, Op2Over),
                (OpCode.OP_2ROT, Op2Rot),
                (OpCode.OP_2SWAP, Op2Swap),


            };
            
            foreach(var opCode in collection)
                _opCodeLookup.Add(opCode.op, opCode.action);

            // OpCodes [0x01, 0x4b] read the next n bytes from
            // the script as data, and push it on the stack.
            for (byte code = 0x01; code <= 0x4b; code++)
            {
                var opCode = (OpCode) code;
                var length = code;
                _opCodeLookup.Add(opCode, () => OpPushBytes(length));
            }
            
            // Opcode with values [0x51-0x60]
            // are aliased as OP_1, OP_2, ... OP_N.
            // Each instruction pushes its N value onto the stack.            
            for (byte code = 0x51; code <= 0x60; code++)
            {
                var opCode = (OpCode) code;
                var value = (byte)(code - 0x50);
                _opCodeLookup.Add(opCode, () => OpPush(value));
            }

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

            try
            {
                action();
            }
            catch (EarlyReturnException e)
            {
                Console.Write(e.Message);
                throw;
            }
        }
    }
}