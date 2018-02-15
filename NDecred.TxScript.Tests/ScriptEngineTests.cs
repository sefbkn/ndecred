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
                var stack = new ScriptStack();
                stack.Push(test.bytes);
                
                var script = new Script(new[]{OpCode.OP_IFDUP});
                var engine = new ScriptEngine(script, stack);
                engine.Run();

                if (test.shouldDuplicate)
                    Assert.Equal(2, engine.MainStack.Count);
                else
                    Assert.Equal(1, engine.MainStack.Count);
            }            
        }


        [Fact]
        public void Run_OpFromAltStack_PushesValueFromAltStackToMainStack()
        {
            var expected = new byte[]{1,2,3};
            var stack = new ScriptStack();
            stack.Push(expected);
            
            var script = new Script(new[]{OpCode.OP_TOALTSTACK, OpCode.OP_FROMALTSTACK});
            var engine = new ScriptEngine(script, stack);
            engine.Run();
            
            Assert.Equal(expected, (IEnumerable<byte>)engine.MainStack.Pop());
        }

        [Fact]
        public void Run_OpToAltStack_PushesValueFromMainStackToAltStack()
        {
            var expected = new byte[]{1,2,3};
            var stack = new ScriptStack();
            stack.Push(expected);
            
            var script = new Script(new[]{OpCode.OP_TOALTSTACK});
            var engine = new ScriptEngine(script, stack);
            engine.Run();
            
            Assert.Equal(expected, (IEnumerable<byte>)engine.AltStack.Pop());
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

                var readBytes = engine.MainStack.Pop();
                
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
                Assert.Equal(index - 0x50, engine.MainStack.PopInt32());
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
            
            Assert.Equal(2, engine.MainStack.PopInt32());
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
            
            Assert.Equal(3, engine.MainStack.PopInt32());
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
            
            Assert.Equal(2, engine.MainStack.PopInt32());
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
                    OpCode.OP_16, 
                OpCode.OP_ENDIF
            });

            var engine = new ScriptEngine(script);
            engine.Run();
            
            Assert.Equal(8, engine.MainStack.PopInt32());
            Assert.Equal(6, engine.MainStack.PopInt32());            
        }

        [Fact]
        public void ScriptEngine_OpSubStr_ReturnsExpectedValues()
        {
            var data = new byte[] { 0, 1, 2, 3, 4 };
            var tests = new (int startIndex, int endIndexExclusive, byte[] expectedValue, Type exception)[]
            {
                // Extracts values, no error
                (1, 1, new byte[]{}, null),
                (4, 4, new byte[]{}, null),
                (0, 1, new byte[]{0}, null),
                (0, 5, new byte[]{0,1,2,3,4}, null),
                (0, 4, new byte[]{0,1,2,3}, null),
                (0, 1, new byte[]{0}, null),
                (2, 4, new byte[]{2,3}, null),
                
                // Behave like go slices.
                (5, 5, new byte[]{}, null),
                
                // Raises exceptions
                (-1, 2, new byte[]{}, typeof(ScriptException)),
                (0, -1, new byte[]{}, typeof(ScriptException)),
                (1, 0, new byte[]{}, typeof(ScriptException)),
                (5, 6, new byte[]{}, typeof(ScriptException)),
                (5, 3, new byte[]{}, typeof(ScriptException)),

                // TODO: Subclass ScriptException and make sure expected errors are thrown
                // in the correct order.
            };

            foreach (var test in tests)
            {
                var script = new Script(new[]
                {
                    OpCode.OP_SUBSTR
                });

                var mainStack = new ScriptStack();
                mainStack.Push(data);
                mainStack.Push(new ScriptInteger(test.endIndexExclusive));
                mainStack.Push(new ScriptInteger(test.startIndex));
                
                var engine = new ScriptEngine(script, mainStack);

                if (test.exception != null)
                {
                    // Run should thow expected exception, and continue.
                    Assert.Throws(test.exception, () => engine.Run());
                    
                    // There should be no items left on the stack
                    Assert.False(engine.MainStack.Any());
                }
                else
                {
                    engine.Run();
                    var result = engine.MainStack.Pop();
                    Assert.Equal(test.expectedValue, (IEnumerable<byte>) result);
                }
            }
        }
    }
}