namespace NDecred.Wire
{
    public abstract class Message : NetworkEncodable
    {
        public abstract MsgCommand Command { get; }
    }
}