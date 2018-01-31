using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.Core
{
    public class TxWitness
    {
        public TxWitness(byte[][] value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
        
        public byte[][] Value { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is TxWitness b)) return false;
            if (Value.Length != b.Value.Length) return false;

            for(var i = 0; i < Value.Length; i++)
                if (!Value[i].SequenceEqual(b.Value[i]))
                    return false;
            
            return true;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}