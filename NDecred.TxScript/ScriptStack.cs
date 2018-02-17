using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public class ScriptStack : List<byte[]>
    {
        public const int MaxByteArrayLength = 520;
        
        public void Push(ScriptInteger scriptInteger)
        {
            Push(scriptInteger.ToBytes());
        }

        public void Push(byte[] data)
        {
            if(data.Length > MaxByteArrayLength)
                throw new ScriptException($"Stack element must not be larger than {MaxByteArrayLength} bytes");
            
            this.Insert(0, data);
        }

        public void Push(bool condition)
        {
            var data = condition ? new byte[] {1} : new byte[0];
            Push(data);
        }

        public byte[] Pop()
        {
            var value = this[0];
            RemoveAt(0);
            return value;
        }

        public byte[] Peek()
        {
            return this[0];
        }
        
        public int PopInt32()
        {
            return (int) new ScriptInteger(Pop(), true, ScriptInteger.MathOpcodeMaxLength);
        }

        public bool PopBool()
        {
            var val = Pop();

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