using System;
using System.Linq;
using Xunit;

namespace NDecred.TxScript.Tests
{
    public class TxScriptTests
    {        
        public TxScriptTests()
        {
        }
        
        [Fact]
        public void ScriptEngine_GivenNestedConditions_LeavesExpectedValuesOnStack()
        {
            var opCodes = new[]
            {
                OpCode.OP_1,
                OpCode.OP_IF,
                    OpCode.OP_0,
                    OpCode.OP_IF, 
                        OpCode.OP_0, 
                    OpCode.OP_ELSE,
                        OpCode.OP_1,
                        OpCode.OP_IF,
                            OpCode.OP_1, // Should reach here.
                        OpCode.OP_ELSE,
                            OpCode.OP_1,
                        OpCode.OP_ENDIF,
                        OpCode.OP_0, // and here
                    OpCode.OP_ENDIF,
                OpCode.OP_ELSE,
                    OpCode.OP_0, 
                OpCode.OP_ENDIF
            };

            var engine = new ScriptEngine();
            engine.Run(opCodes);
            
            Assert.Equal(2, engine.DataStack.Count);
            Assert.Equal(1, engine.BranchStack.Count);
            Assert.Equal(false, engine.DataStack.PopBool());
            Assert.Equal(true, engine.DataStack.PopBool());            
        }
    }
}