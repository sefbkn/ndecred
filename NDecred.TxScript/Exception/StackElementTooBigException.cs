using System;

namespace NDecred.TxScript
{
    public class StackElementTooBigException : ScriptException
    {
        public int Value { get; }
        public int MaxValue { get; }
        
        public StackElementTooBigException(int actualValue, int maxValue) : base("Stack element size larger than max allowed value")
        {
            Value = actualValue;
            MaxValue = maxValue;
        }
    }
}