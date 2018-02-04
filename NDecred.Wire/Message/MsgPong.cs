using System.IO;

namespace NDecred.Wire
{
    public class MsgPong : NetworkEncodable
    {
        public ulong Nonce { get; set; }

        public override void Decode(BinaryReader reader)
        {
            Nonce = reader.ReadUInt64();
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(Nonce);
        }
    }
}