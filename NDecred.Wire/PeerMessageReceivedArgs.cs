using System;

namespace NDecred.Wire
{
    public class PeerMessageReceivedArgs<T> : EventArgs where T : Message
    {
        public MessageHeader Header { get; }
        public T Message { get; }

        public PeerMessageReceivedArgs(MessageHeader header, T message)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

    }
    
    public class PeerMessageReceivedArgs : PeerMessageReceivedArgs<Message>
    {
        public PeerMessageReceivedArgs(MessageHeader header, Message message) : base(header, message)
        {
        }
    }
}