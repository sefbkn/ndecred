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
    
    /// <summary>
    /// Contains an incoming messaged received by a Peer instance, 
    /// and passed when a MessageReceived event is fired.
    /// </summary>
    public class PeerMessageReceivedArgs : PeerMessageReceivedArgs<Message>
    {
        public PeerMessageReceivedArgs(MessageHeader header, Message message) : base(header, message)
        {
        }
    }
}