using System.IO;

namespace NDecred.Network
{
    // MsgVerAck defines a decred verack message which is used for a peer to
    // acknowledge a version message (MsgVersion) after it has used the information
    // to negotiate parameters.  It implements the Message interface.
    //
    // This message has no payload.

    public class MsgVerAck : NetworkEncodable
    {
        public override void Decode(BinaryReader reader)
        {
        }

        public override void Encode(BinaryWriter writer)
        {
        }
    }
}