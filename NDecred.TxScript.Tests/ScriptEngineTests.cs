using System;
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
        public void Run_WithSimpleIfConditionFalse_ExecutesElseBlock()
        {
            var opCodes = new[]
            {
                OpCode.OP_FALSE,
                OpCode.OP_IF,
                OpCode.OP_2,
                OpCode.OP_ELSE,
                OpCode.OP_3, 
                OpCode.OP_ENDIF
            };

            var engine = new ScriptEngine();
            engine.Run(opCodes);
            
            Assert.Equal(3, engine.DataStack.PopInt32());
        }

        [Fact]
        public void Run_WithSimpleIfConditionTrue_ExecutesIfBlock()
        {
            var opCodes = new[]
            {
                OpCode.OP_TRUE,
                OpCode.OP_IF,
                OpCode.OP_2,
                OpCode.OP_ELSE,
                OpCode.OP_3, 
                OpCode.OP_ENDIF
            };

            var engine = new ScriptEngine();
            engine.Run(opCodes);
            
            Assert.Equal(2, engine.DataStack.PopInt32());
        }
        
        [Fact]
        public void ScriptEngine_GivenNestedConditions_LeavesExpectedValuesOnStack()
        {
            var opCodes = new[]
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
            };

            var engine = new ScriptEngine();
            engine.Run(opCodes);
            
            Assert.Equal(8, engine.DataStack.PopInt32());
            Assert.Equal(6, engine.DataStack.PopInt32());            
        }
    }
}