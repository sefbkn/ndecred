using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public partial class ScriptEngine
    {
        private readonly Dictionary<OpCode, Action<ParsedOpCode>> _opCodeLookup;

        private bool _hasRun;
        private readonly object _lock = new object();
        private Script Script { get; }
        private BranchStack BranchStack { get; }

        public ScriptEngine(Script script, ScriptStack mainStack = null, BranchStack branchStack = null)
        {
            Script = script;
            MainStack = mainStack ?? new ScriptStack();
            BranchStack = branchStack ?? new BranchStack();
            AltStack = new ScriptStack();
            _opCodeLookup = new Dictionary<OpCode, Action<ParsedOpCode>>();

            InitializeOpCodeDictionary();
        }
        
        public ScriptStack AltStack { get; }
        public ScriptStack MainStack { get; }
        
        /// <summary>
        /// Executes a sequence of instructions, taking branching logic into account.
        /// </summary>
        public void Run()
        {
            lock (_lock)
            {
                if (_hasRun)
                {
                    throw new InvalidOperationException(
                        $"Run method can only be called once on a {nameof(ScriptEngine)} instance.");
                }

                _hasRun = true;
            }

            foreach (var op in Script.ParsedOpCodes)
            {
                var branchOp = BranchStack.Peek();
                
                // Ensure that disabled and reserved opcodes are executed.
                var canExecute = 
                    branchOp == BranchOption.True 
                 || op.Code.IsConditional()
                 || op.Code.IsDisabled()
                 || op.Code.IsReserved();
                
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

        private void InitializeOpCodeDictionary()
        {
            var collection = new (OpCode op, Action<ParsedOpCode> action)[]
            {
                (OpCode.OP_0, op => OpFalse()),
                (OpCode.OP_1NEGATE, op => OpPush(-1)),
                (OpCode.OP_VER, OpReserved),
                (OpCode.OP_VERIF, OpReserved),
                (OpCode.OP_VERNOTIF, OpReserved),
                (OpCode.OP_NOP, OpNop),
                (OpCode.OP_IF, op => OpIf()),
                (OpCode.OP_NOTIF, op => OpNotIf()),
                (OpCode.OP_ELSE, op => OpElse()),
                (OpCode.OP_ENDIF, op => OpEndIf()),
                (OpCode.OP_VERIFY, op => OpVerify()),
                (OpCode.OP_RETURN, op => OpReturn()),
                (OpCode.OP_PUSHDATA1, OpPushData),
                (OpCode.OP_PUSHDATA2, OpPushData),
                (OpCode.OP_PUSHDATA4, OpPushData),
                (OpCode.OP_RESERVED, OpReserved),
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
                (OpCode.OP_CODESEPARATOR, OpDisabled), // should not be executed.
                (OpCode.OP_CAT, OpCat),
                (OpCode.OP_SUBSTR, OpSubStr),
                (OpCode.OP_LEFT, OpLeft),
                (OpCode.OP_RIGHT, OpRight),
                (OpCode.OP_SIZE              , OpSize),
                (OpCode.OP_INVERT            , OpInvert),
                (OpCode.OP_AND               , OpAnd),
                (OpCode.OP_OR                , OpOr),
                (OpCode.OP_XOR               , OpXor),
                (OpCode.OP_EQUAL             , OpEqual),
                (OpCode.OP_EQUALVERIFY       , OpEqualVerify),
                (OpCode.OP_ROTR              , OpRotr),
                (OpCode.OP_ROTL              , OpRotl),
                (OpCode.OP_1ADD              , Op1Add),
                (OpCode.OP_1SUB              , Op1Sub),
                (OpCode.OP_2MUL              , OpNop), // disabled.
                (OpCode.OP_2DIV              , OpNop), // disabled
                (OpCode.OP_NEGATE            , OpNegate),
                (OpCode.OP_ABS               , OpAbs),
                (OpCode.OP_NOT               , OpNot),
                (OpCode.OP_0NOTEQUAL         , Op0NotEqual),
                (OpCode.OP_ADD               , OpAdd),
                (OpCode.OP_SUB               , OpSub),
                (OpCode.OP_MUL               , OpMul),
                (OpCode.OP_DIV               , OpDiv),
                (OpCode.OP_MOD               , OpMod),
                (OpCode.OP_LSHIFT            , OpLShift),
                (OpCode.OP_RSHIFT            , OpRShift),
                (OpCode.OP_BOOLAND           , OpBoolAnd),
                (OpCode.OP_BOOLOR            , OpBoolOr),
                (OpCode.OP_NUMEQUAL          , OpNumEqual),
                (OpCode.OP_NUMEQUALVERIFY    , OpNumEqualVerify),
                (OpCode.OP_NUMNOTEQUAL       , OpNumNotEqual),
                (OpCode.OP_LESSTHAN          , OpLessThan),
                (OpCode.OP_GREATERTHAN       , OpGreaterThan),
                (OpCode.OP_LESSTHANOREQUAL   , OpLessThanOrEqual),
                (OpCode.OP_GREATERTHANOREQUAL, OpGreaterThanOrEqual),
                (OpCode.OP_MIN               , OpMin),
                (OpCode.OP_MAX               , OpMax),
                (OpCode.OP_WITHIN            , OpWithin),
                (OpCode.OP_RIPEMD160         , OpRipemd160),
                (OpCode.OP_SHA1              , OpSha1),
                (OpCode.OP_BLAKE256          , OpBlake256),
                (OpCode.OP_HASH160           , OpHash160),
                (OpCode.OP_HASH256           , OpHash256) 
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