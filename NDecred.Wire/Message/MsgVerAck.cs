using System.IO;

namespace NDecred.Wire
{
    public class MsgVerAck : Message
    {
        public override void Decode(BinaryReader reader)
        {
        }

        public override void Encode(BinaryWriter writer)
        {
        }
        
        public override MsgCommand Command => MsgCommand.VerAck;
    }
}