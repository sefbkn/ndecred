namespace NDecred.Core
{
    public abstract class BaseEncoding
    {
        public abstract string Encode(byte[] bytes);
        public abstract byte[] Decode(string value);
    }
}