using Newtonsoft.Json;

namespace NDecred.Wire
{
    /// <summary>
    /// Base class that all messages sent over the wire inherit from
    /// </summary>
    public abstract class Message : NetworkEncodable
    {
        // The command that a message corresponds to.
        // Used to:
        //   * determine how to decode a received message
        //   * write the message type when encoding a message.
        public abstract MsgCommand Command { get; }
        
        public NetworkEncodable Clone()
        {
            var val = JsonConvert.SerializeObject(this);
            return (NetworkEncodable) JsonConvert.DeserializeObject(val, GetType());            
        }
    }
}