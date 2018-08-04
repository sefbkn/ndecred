using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public class ScriptStack
    {
        public const int MaxByteArrayLength = 520;
        private readonly List<byte[]> _stack = new List<byte[]>();

        public int Size() => _stack.Count;
        
        public void Push(ScriptInteger scriptInteger)
        {
            Push(scriptInteger.ToBytes());
        }

        public void Push(byte[] data, int depth = 0)
        {
            if(data.Length > MaxByteArrayLength)
                throw new StackElementTooBigException(data.Length, MaxByteArrayLength);

            var index = _stack.Count - depth;
            _stack.Insert(index, data);
        }

        public void Push(bool condition)
        {
            var data = condition ? new byte[] {1} : new byte[0];
            Push(data);
        }

        public byte[] Pop(int depth = 0)
        {
            if(_stack.Count == 0)
                throw new EmptyScriptStackException();
            
            var value = _stack[_stack.Count - (1 + depth)];
            _stack.RemoveAt(_stack.Count - (1 + depth));
            return value;
        }

        public byte[] Peek(int depth = 0)
        {
            return _stack[_stack.Count - (1 + depth)];
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