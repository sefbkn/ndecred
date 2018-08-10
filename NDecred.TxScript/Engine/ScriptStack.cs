using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    public class ScriptStack
    {
        public int Size() => _stack.Count;
        private readonly bool _minimalEncoding;
        private readonly List<byte[]> _stack = new List<byte[]>();

        public ScriptStack(bool minimalEncoding)
        {
            _minimalEncoding = minimalEncoding;
        }

        public void Push(int value)
        {
            var scriptInt = new ScriptInteger(value);
            Push(scriptInt.ToBytes());
        }

        public void Push(byte[] data, int depth = 0)
        {
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
            var index = _stack.Count - (1 + depth);

            if(depth < 0)
                throw new InvalidOperationException();
            if(_stack.Count == 0)
                throw new EmptyScriptStackException();

            var value = _stack[index];
            _stack.RemoveAt(index);
            return value;
        }

        public byte[][] PopN(int count)
        {
            return Enumerable.Range(0, count).Select(i => Pop()).ToArray();
        }

        public byte[] Peek(int depth = 0)
        {
            return _stack[_stack.Count - (1 + depth)];
        }

        public int PopInt32()
        {
            var scriptInt = new ScriptInteger(Pop(), _minimalEncoding, ScriptInteger.MathOpcodeMaxLength);
            return (int) scriptInt.Value;
        }

        public bool PeekBool()
        {
            return BytesToBool(Peek());
        }

        public bool PopBool()
        {
            return BytesToBool(Pop());
        }

        private bool BytesToBool(byte[] val)
        {
            // Empty value, then false
            if (val.Length == 0)
                return false;

            // All zeroes, then false
            if (val.All(b => b == 0x00))
                return false;

            // TODO: Is this the correct byte order?
            // If sign bit is the only bit set, then false
            if (val.Last() == 0x80 && val.Take(val.Length - 1).All(b => b == 0x00))
                return false;

            return true;
        }
    }
}
