using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public partial class ScriptEngine
    {
        private bool _hasRun = false;
        private readonly Dictionary<OpCode, Action<ParsedOpCode>> _opCodeLookup;
        
        
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
            _opCodeLookup = new Dictionary<OpCode, Action<ParsedOpCode>>();

            InitializeOpCodeDictionary();
        }
        
        /// <summary>
        /// Executes a sequence of instructions, taking branching logic into account.
        /// </summary>
        public void Run()
        {
            if(_hasRun) 
                throw new InvalidOperationException("Script engine can only be called once.");
            _hasRun = true;
            
            foreach (var op in Script.ParsedOpCodes)
            {
                var branchOp = BranchStack.Peek();
                var canExecute = branchOp == BranchOption.True || op.Code.IsConditional();
                if (!canExecute) continue;
                Execute(op);
            }
            
            if (BranchStack.Count > 1)
            {
                throw new ScriptSyntaxError("Script missing OP_ENDIF");
            }
        }
        
        private void Execute(ParsedOpCode op)
        {
            if (!_opCodeLookup.TryGetValue(op.Code, out var action))
                throw new ScriptSyntaxError($"Attempted to execute unknown op code 0x{(byte)op.Code:X}");
            
            if(op.Code.IsReserved())
                throw new ReservedOpCodeException(op.Code);
            
            action(op);
        }

        public void InitializeOpCodeDictionary()
        {
            var collection = new (OpCode op, Action<ParsedOpCode> action)[]
            {
                (OpCode.OP_0, op => OpFalse()),
                (OpCode.OP_1NEGATE, op => OpPush(-1)),
                (OpCode.OP_VER, op => OpReserved(op)),
                (OpCode.OP_VERIF, op => OpReserved(op)),
                (OpCode.OP_VERNOTIF, op => OpReserved(op)),
                (OpCode.OP_NOP, op => OpNop(op)),
                (OpCode.OP_IF, op => OpIf()),
                (OpCode.OP_NOTIF, op => OpNotIf()),
                (OpCode.OP_ELSE, op => OpElse()),
                (OpCode.OP_ENDIF, op => OpEndIf()),
                (OpCode.OP_VERIFY, op => OpVerify()),
                (OpCode.OP_RETURN, op => OpReturn()),
                (OpCode.OP_PUSHDATA1, op => OpPushData(op)),
                (OpCode.OP_PUSHDATA2, op => OpPushData(op)),
                (OpCode.OP_PUSHDATA4, op => OpPushData(op)),
                (OpCode.OP_RESERVED, op => OpReserved(op)),
                (OpCode.OP_TOALTSTACK, op => OpToAltStack()),
                (OpCode.OP_FROMALTSTACK, op => OpFromAltStack()),
                (OpCode.OP_IFDUP, op => OpIfDup()),
                (OpCode.OP_DEPTH, op => OpDepth()),
                (OpCode.OP_DROP, op => OpDrop()),
                (OpCode.OP_DUP, op => OpDup()),
                (OpCode.OP_NIP, op => OpNip()),
                (OpCode.OP_OVER, op => OpOver()),
                (OpCode.OP_PICK, op => OpPick()),
                (OpCode.OP_ROLL, op => OpRoll()),
                (OpCode.OP_ROT, op => OpRot()),
                (OpCode.OP_SWAP, op => OpSwap()),
                (OpCode.OP_TUCK, op => OpTuck()),
                (OpCode.OP_2DROP, op => Op2Drop()),
                (OpCode.OP_2DUP, op => Op2Dup()),
                (OpCode.OP_3DUP, op => Op3Dup()),
                (OpCode.OP_2OVER, op => Op2Over()),
                (OpCode.OP_2ROT, op => Op2Rot()),
                (OpCode.OP_2SWAP, op => Op2Swap()),
                (OpCode.OP_CODESEPARATOR, OpDisabled)
            };
            
            
            foreach(var opCode in collection)
                _opCodeLookup.Add(opCode.op, opCode.action);

            for (var code = OpCode.OP_DATA_1; code <= OpCode.OP_DATA_75; code++)
                _opCodeLookup.Add(code, OpPushBytes);
            for (var code = OpCode.OP_1; code <= OpCode.OP_16; code++)
                _opCodeLookup.Add(code, OpPushBytes);
            for (var code = OpCode.OP_UNKNOWN193; code <= OpCode.OP_UNKNOWN248; code++)
                _opCodeLookup.Add(code, OpNop);
        }
    }
}