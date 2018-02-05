using System;

namespace NDecred.Wire
{
    public class PeerMessageReceivedArgs : EventArgs
    {
        public MessageHeader Header { get; }
        public Message Message { get; }

        public PeerMessageReceivedArgs(MessageHeader header, Message message)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}