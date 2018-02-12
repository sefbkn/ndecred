using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NDecred.TxScript.Tests
{
    public class ScriptEngineTests
    {
        public ScriptEngineTests()
        {
        }

        [Fact]
        public void Run_OpPushData_PushesExpectedDataOntoStack()
        {
            var tests = new(OpCode op, byte[] len, IEnumerable<int> data)[]
            {
                (OpCode.OP_PUSHDATA1, new byte[]{0}, Enumerable.Repeat(1, 0)),
                (OpCode.OP_PUSHDATA1, new byte[]{1}, Enumerable.Repeat(1, 1)),
                (OpCode.OP_PUSHDATA1, new byte[]{0xff}, Enumerable.Repeat(1, 0xff)),
                (OpCode.OP_PUSHDATA2, new byte[]{0x00, 0x01}, Enumerable.Repeat(1, 0x0100)),
                (OpCode.OP_PUSHDATA2, new byte[]{0x00, 0x02}, Enumerable.Repeat(1, 0x0200)),
                (OpCode.OP_PUSHDATA4, new byte[]{0x00, 0x00, 0x00, 0x00}, Enumerable.Repeat(1, 0)),
                (OpCode.OP_PUSHDATA4, new byte[]{0xff, 0x01, 0x00, 0x00}, Enumerable.Repeat(1, 0x01ff)),
                (OpCode.OP_PUSHDATA4, new byte[]{0xff, 0x00, 0x00, 0x00}, Enumerable.Repeat(1, 0xff)),
            };

            foreach (var test in tests)
            {
                var data = test.data.Select(i => (byte) i).ToArray();

                var rawScript = new[]{(byte) test.op}
                    .Concat(test.len)
                    .Concat(data)
                    .ToArray();
                
                var script = new Script(rawScript);
                var engine = new ScriptEngine(script);
                engine.Run();

                var readBytes = engine.DataStack.PopBytes();
                
                Assert.Equal(rawScript.Length, engine.InstructionPointer);
                Assert.Equal(data.Length, readBytes.Length);
                Assert.Equal(data, (IEnumerable<byte>) readBytes);
            }
        }
        
        [Fact]
        public void Run_OpReturn_ThrowsEarlyReturnException()
        {
            var script = new Script(new[]{ OpCode.OP_RETURN });
            var engine = new ScriptEngine(script);
            Assert.Throws<EarlyReturnException>(() => engine.Run());
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
                var script = new Script(new[]{opCode});
                var engine = new ScriptEngine(script);
                Assert.Throws<ReservedOpCodeException>(() => engine.Run());
            }
        }
        
        [Fact]
        public void Run_PushIntegerOpCodes_PushesRespectiveIntegerOntoDataStack()
        {
            for (var index = 0x51; index <= 0x60; index++)
            {
                var opCode = (OpCode) index;
                var script = new Script(new[]{opCode});
                var engine = new ScriptEngine(script);
                engine.Run();
                Assert.Equal(index - 0x50, engine.DataStack.PopInt32());
            }
        }

        [Fact]
        public void Run_WithSimpleNotIfConditionTrue_ExecutesNotIfBlock()
        {
            var script = new Script(new[]
            {
                OpCode.OP_FALSE,
                OpCode.OP_NOTIF,
                OpCode.OP_2,
                OpCode.OP_ELSE,
                OpCode.OP_3, 
                OpCode.OP_ENDIF
            });

            var engine = new ScriptEngine(script);
            engine.Run();
            
            Assert.Equal(2, engine.DataStack.PopInt32());
        }
        
        [Fact]
        public void Run_WithSimpleIfConditionFalse_ExecutesElseBlock()
        {
            var script = new Script(new[]
            {
                OpCode.OP_FALSE,
                OpCode.OP_IF,
                OpCode.OP_2,
                OpCode.OP_ELSE,
                OpCode.OP_3, 
                OpCode.OP_ENDIF
            });

            var engine = new ScriptEngine(script);
            engine.Run();
            
            Assert.Equal(3, engine.DataStack.PopInt32());
        }

        [Fact]
        public void Run_WithSimpleIfConditionTrue_ExecutesIfBlock()
        {
            var script = new Script(new[]
            {
                OpCode.OP_TRUE,
                OpCode.OP_IF,
                OpCode.OP_2,
                OpCode.OP_ELSE,
                OpCode.OP_3, 
                OpCode.OP_ENDIF
            });

            var engine = new ScriptEngine(script);
            engine.Run();
            
            Assert.Equal(2, engine.DataStack.PopInt32());
        }
        
        [Fact]
        public void ScriptEngine_GivenNestedConditions_LeavesExpectedValuesOnStack()
        {
            var script = new Script(new[]
            {
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
                    OpCode.OP_ENDIF,
                OpCode.OP_ELSE,
                    OpCode.OP_10, 
                OpCode.OP_ENDIF
            });

            var engine = new ScriptEngine(script);
            engine.Run();
            
            Assert.Equal(8, engine.DataStack.PopInt32());
            Assert.Equal(6, engine.DataStack.PopInt32());            
        }
    }
}