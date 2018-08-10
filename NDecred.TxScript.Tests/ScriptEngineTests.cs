using System;
using System.Collections.Generic;
using System.Linq;
using NDecred.Cryptography;
using NDecred.Wire;
using Xunit;


namespace NDecred.TxScript.Tests
{
    public class ScriptEngineTests
    {
        private MsgTx _transaction = new MsgTx
        {
            Expiry = 0,
            LockTime = 0,
            ProtocolVersion = 0,
            SerializationType = TxSerializeType.Full,
            TxIn = new []
            {
                new TxIn
                {
                    BlockHeight = 0,
                    BlockIndex = 0,
                    PreviousOutPoint = new OutPoint(new byte[32], 0, TxTree.TxTreeRegular)
                }
            },
            TxOut = new []
            {
                new TxOut
                {
                    PkScript = new byte[0],
                    Value = 0,
                    Version = 0
                }
            }
        };

        public ScriptEngineTests()
        {
        }

        private ScriptEngine GetDefaultEngine(Script script)
        {
            var options = new ScriptOptions();
            return new ScriptEngine(_transaction, 0, script, options);
        }

        [Fact]
        public void Run_OpIfDup_DuplicatesIfNotZero()
        {
            var tests = new(bool shouldDuplicate, byte[] bytes)[]
            {
                (true, new byte[]{1}),
                (true, new byte[]{1,2,3,4,5,6,0,0,0}),
                (false, new byte[]{}),
                (false, new byte[]{0})
            };

            foreach (var test in tests)
            {
                var script = new ScriptBuilder(
                    test.bytes,
                    OpCode.OP_IFDUP,
                    OpCode.OP_TRUE
                ).ToScript();

                var engine = GetDefaultEngine(script);
                engine.Run();

                if (test.shouldDuplicate)
                {
                    Assert.Equal(engine.MainStack.Pop(), (IEnumerable<byte>) test.bytes);
                    Assert.Equal(engine.MainStack.Pop(), (IEnumerable<byte>) test.bytes);
                }

                else
                {
                    Assert.Equal(false, engine.MainStack.PopBool());
                }
            }
        }

        [Fact]
        public void Run_OpFromAltStack_PushesValueFromAltStackToMainStack()
        {
            var expected = new byte[]{1,2,3};
            var script = new ScriptBuilder(
                expected,
                OpCode.OP_TOALTSTACK,
                OpCode.OP_FROMALTSTACK,
                OpCode.OP_TRUE
            ).ToScript();

            var engine = GetDefaultEngine(script);
            engine.Run();

            Assert.Equal(expected, (IEnumerable<byte>)engine.MainStack.Pop());
        }

        [Fact]
        public void Run_OpToAltStack_PushesValueFromMainStackToAltStack()
        {
            var expected = new byte[]{1,2,3};
            var script = new ScriptBuilder(
                expected,
                OpCode.OP_TOALTSTACK,
                OpCode.OP_TRUE
            ).ToScript();

            var engine = GetDefaultEngine(script);
            engine.Run();

            Assert.Equal(0, engine.MainStack.Size());
            Assert.Equal(1, engine.AltStack.Size());
            Assert.Equal(expected, (IEnumerable<byte>)engine.AltStack.Pop());
        }

        [Fact]
        public void Run_OpReturn_ThrowsEarlyReturnException()
        {
            var script = new ScriptBuilder(OpCode.OP_RETURN).ToScript();
            var engine = GetDefaultEngine(script);
            var result = engine.Run();
            Assert.False(result);
        }

        [Fact]
        public void Run_GivenReservedOpCode_ThrowsReservedOpCodeException()
        {
            var reserved = new[]
            {
                OpCode.OP_VER,
                OpCode.OP_VERIF,
                OpCode.OP_VERNOTIF
            };

            foreach (var opCode in reserved)
            {
                var script = new ScriptBuilder(opCode).ToScript();
                var engine = GetDefaultEngine(script);
                var result = engine.Run();
                Assert.False(result);
            }
        }

        [Fact]
        public void Run_PushIntegerOpCodes_PushesRespectiveIntegerOntoDataStack()
        {
            for (var opCode = OpCode.OP_1; opCode <= OpCode.OP_16; opCode++)
            {
                var script = new ScriptBuilder(opCode, OpCode.OP_TRUE).ToScript();
                var engine = GetDefaultEngine(script);
                engine.Run();
                Assert.Equal(opCode - OpCode.OP_RESERVED, engine.MainStack.PopInt32());
            }
        }

        [Fact]
        public void Run_WithSimpleNotIfConditionTrue_ExecutesNotIfBlock()
        {
            var script = new ScriptBuilder(
                OpCode.OP_FALSE,
                OpCode.OP_NOTIF,
                OpCode.OP_2,
                OpCode.OP_TRUE,
                OpCode.OP_ELSE,
                OpCode.OP_3,
                OpCode.OP_ENDIF
            ).ToScript();

            var engine = GetDefaultEngine(script);
            engine.Run();

            Assert.Equal(2, engine.MainStack.PopInt32());
        }

        [Fact]
        public void Run_WithSimpleIfConditionFalse_ExecutesElseBlock()
        {
            var script = new ScriptBuilder(
                OpCode.OP_FALSE,
                OpCode.OP_IF,
                OpCode.OP_2,
                OpCode.OP_ELSE,
                OpCode.OP_3,
                OpCode.OP_TRUE,
                OpCode.OP_ENDIF
            ).ToScript();

            var engine = GetDefaultEngine(script);
            engine.Run();

            Assert.Equal(3, engine.MainStack.PopInt32());
        }

        [Fact]
        public void Run_WithSimpleIfConditionTrue_ExecutesIfBlock()
        {
            var script = new ScriptBuilder(
                OpCode.OP_TRUE,
                OpCode.OP_IF,
                OpCode.OP_2,
                OpCode.OP_TRUE,
                OpCode.OP_ELSE,
                OpCode.OP_3,
                OpCode.OP_ENDIF
            ).ToScript();

            var engine = GetDefaultEngine(script);
            engine.Run();

            Assert.Equal(2, engine.MainStack.PopInt32());
        }

        [Fact]
        public void ScriptEngine_GivenNestedConditions_LeavesExpectedValuesOnStack()
        {
            var script = new ScriptBuilder(
                OpCode.OP_TRUE,
                OpCode.OP_IF,
                    OpCode.OP_0,
                    OpCode.OP_IF,
                        OpCode.OP_0,
                    OpCode.OP_ELSE,
                        OpCode.OP_TRUE,
                        OpCode.OP_IF,
                            OpCode.OP_6, // Should reach here.
                        OpCode.OP_ELSE,
                            OpCode.OP_3,
                        OpCode.OP_ENDIF,
                        OpCode.OP_8, // and here
                        OpCode.OP_TRUE,
                    OpCode.OP_ENDIF,
                OpCode.OP_ELSE,
                    OpCode.OP_16,
                OpCode.OP_ENDIF
            ).ToScript();

            var engine = GetDefaultEngine(script);
            engine.Run();

            Assert.Equal(8, engine.MainStack.PopInt32());
            Assert.Equal(6, engine.MainStack.PopInt32());
        }

        [Fact]
        public void ScriptEngine_OpSubStr_ReturnsExpectedValues()
        {
            var data = new byte[] { 0, 1, 2, 3, 4 };
            var tests = new (int startIndex, int endIndexExclusive, byte[] expectedValue, bool success)[]
            {
                // Extracts values, no error
                (1, 1, new byte[]{}, true),
                (4, 4, new byte[]{}, true),
                (0, 1, new byte[]{0}, true),
                (0, 5, new byte[]{0,1,2,3,4}, true),
                (0, 4, new byte[]{0,1,2,3}, true),
                (0, 1, new byte[]{0}, true),
                (2, 4, new byte[]{2,3}, true),

                // Behave like go slices.
                (5, 5, new byte[]{}, true),

                // Raises exceptions
                (-1, 2, new byte[]{}, false),
                (0, -1, new byte[]{}, false),
                (1, 0, new byte[]{},  false),
                (5, 6, new byte[]{},  false),
                (5, 3, new byte[]{},  false),
            };

            foreach (var test in tests)
            {
                var script = new ScriptBuilder(
                    data,
                    test.endIndexExclusive,
                    test.startIndex,
                    OpCode.OP_SUBSTR,
                    OpCode.OP_TRUE
                ).ToScript();

                var engine = GetDefaultEngine(script);
                var result = engine.Run();

                Assert.Equal(test.success, result);

                if (!test.success)
                {
                    // There should be no items left on the stack
                    Assert.Equal(0, engine.MainStack.Size());
                }
                else
                {
                    var topStackElement = engine.MainStack.Pop();
                    Assert.Equal(test.expectedValue.AsEnumerable(), topStackElement.AsEnumerable());
                }
            }
        }
    }
}
