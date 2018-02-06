using System.IO;

namespace NDecred.Wire
{
    /// <summary>
    /// Represents a request sent to other nodes for a MsgMiningState message 
    /// </summary>
    public class MsgGetMiningState : Message
    {
        public override void Decode(BinaryReader reader) { }

        public override void Encode(BinaryWriter writer) { }

        public override MsgCommand Command => MsgCommand.GetMiningState;
    }
}