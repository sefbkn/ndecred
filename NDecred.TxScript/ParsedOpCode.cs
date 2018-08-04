using System;
using System.Collections.Generic;

namespace NDecred.TxScript
{
    public class ParsedOpCode
    {
	    private static readonly byte[] NullBytes = new byte[0];
	    
        public OpCode Code { get; }
        public byte[] Data { get; }

	    public ParsedOpCode(OpCode code, byte[] data = null)
	    {
		    Code = code;
		    Data = data ?? NullBytes;

		    if (!IsCanonicalPush())
			    throw new NonCanonicalOpCodeException(code, data);
	    }
	    
	    /// <summary>
	    /// Determines if the operation is using the smallest amount of data possible.
	    /// 
	    /// Using an OP_PUSHDATA4 to represent a single 0x01 byte of data is wasteful, for example.
	    /// </summary>
	    /// <returns>a boolean representing whether or not the opcode is appropriate for pushing the given data.</returns>
        public bool IsCanonicalPush()
        {
            // If this is not a push instruction.
            if (Code > OpCode.OP_16) return true;
            
            // If an OP_DATA opcode is used to store a value that could have been encoded with an Op_1-16 instruction,
            // return false;
            if (Code >= OpCode.OP_DATA_1 && Code <= OpCode.OP_DATA_75 && Data.Length == 1 && Data[0] <= 16) 
                return false;
            
            // If an opcode that uses less bytes
            // should have been used to store data, return false.
            switch (Code)
            {
                case OpCode.OP_PUSHDATA1 when Data.Length <= (byte) OpCode.OP_DATA_75:
                case OpCode.OP_PUSHDATA2 when Data.Length <= 0xff:
                case OpCode.OP_PUSHDATA4 when Data.Length <= 0xffff:
                    return false;
            }

            return true;
        }
	    
	    /// <summary>
	    /// Determines the expected number bytes an opcode takes up
	    /// without data.
	    /// </summary>
	    /// <returns></returns>
	    private int SerializedLength()
	    {
		    if (Code.IsOpN())
			    return 1;
		    
		    if (Code.IsOpData())
			    return (int) Code + 1;
		    
		    if (Code.IsPushDataOpCode())
		    {
			    switch (Code)
			    {
				    case OpCode.OP_PUSHDATA1:
					    return 1 + 1 + Data.Length;
				    case OpCode.OP_PUSHDATA2:
					    return 1 + 2 + Data.Length;
				    case OpCode.OP_PUSHDATA4:
					    return 1 + 4 + Data.Length;
			    }
		    }
		    
		    return 1;
	    }
	    
	    // Serialize the opcode to a raw representation.
	    public byte[] Serialize()
	    {
		    var serializedLength = SerializedLength();
		    var bytes = new List<byte>(serializedLength) { (byte) Code };

		    if(serializedLength == 1 && Data.Length != 0)
			    throw new ScriptSyntaxErrorException(Code, "Parsed OpCode has data attached although it should not");

		    switch (Code)
		    {
				case OpCode.OP_PUSHDATA1:
					bytes.Add((byte) Data.Length);
					break;
				case OpCode.OP_PUSHDATA2:
					bytes.AddRange(BitConverter.GetBytes((short) Data.Length));
					break;
				case OpCode.OP_PUSHDATA4:
					bytes.AddRange(BitConverter.GetBytes(Data.Length));
					break;
		    }
		    
		    bytes.AddRange(Data);
		    
		    if(bytes.Count != serializedLength)
			    throw new ScriptSyntaxErrorException(Code, $"Serialized OpCode has length {bytes.Count} bytes but should be {serializedLength} bytes");
		    
		    return bytes.ToArray();
	    }

        public override string ToString()
        {
            return Code.ToString();
        }
    }
}