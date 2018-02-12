using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public class Script
    {
        public byte[] Bytes { get; }

        public Script(IEnumerable<OpCode> opCodes)
        {
            Bytes = opCodes.Select(op => (byte) op).ToArray();
        }
        
        public Script(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}