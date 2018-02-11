using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public class ScriptStack
    {
        public const int MaxByteArrayLength = 520;
        
        private Stack<byte[]> Stack { get; }
        public int Count => Stack.Count;
        
        public ScriptStack()
        {
            Stack = new Stack<byte[]>();
            if(Stack.Any(b => b.Length > MaxByteArrayLength))
                throw new InvalidOperationException($"Stack element must not be larger than {MaxByteArrayLength} bytes");
        }

        public ScriptStack Push(byte[] data)
        {
            if(data.Length > MaxByteArrayLength)
                throw new InvalidOperationException($"Stack element must not be larger than {MaxByteArrayLength} bytes");
            Stack.Push(data);
            return this;
        }

        public ScriptInteger PopInteger(int maxValue)
        {
            return new ScriptInteger(Stack.Pop(), maxValue);
        }

        public byte PopByte()
        {
            var val = Stack.Pop();
            if(val.Length != 1)
                throw new InvalidOperationException("Expected value of length 1 to be on stack");
            return val[0];
        }

        public bool PopBool()
        {
            var val = Stack.Pop();

            // Empty value, then false
            if (val.Length == 0)
                return false;
            // All zeroes, then false
            if (val.All(b => b == 0x00))
                return false;
            // If sign bit is the only bit set, then false
            if (val.Last() == 0x80 &&
                val.Take(val.Length - 1).All(b => b == 0x00))
                return false;

            return true;
        }
    }
}