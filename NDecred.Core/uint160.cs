namespace NDecred.Core
{
    public class uint160 : uintn
    {
        public uint160(string value) : base(20, value)
        {
        }

        public uint160(byte[] value) : base(20, value)
        {
        }

        public uint160(uint160 value) : this(value.Bytes)
        {
        }
    }
}