using System.IO;
using System.Linq;
using NDecred.Common;

namespace NDecred.Network
{
    public abstract class Message : NetworkEncodable
    {
        public static readonly int ChecksumLengthBytes = 4;

        public Message()
        {
        }

        public Message(CurrencyNet currencyNet, Command command, byte[] payload)
        {
            CurrencyNetwork = currencyNet;
            Command = command;
            PayloadLength = payload.Length;
            Checksum = Hash.BLAKE256(payload).Take(ChecksumLengthBytes).ToArray();
        }

        public CurrencyNet CurrencyNetwork { get; set; }
        public Command Command { get; set; }
        public int PayloadLength { get; set; }
        public byte[] Checksum { get; set; }

        public override void Decode(BinaryReader reader)
        {
            CurrencyNetwork = (CurrencyNet) reader.ReadUInt32();

            Command = new Command();
            Command.Decode(reader);

            PayloadLength = reader.ReadInt32();
            Checksum = reader.ReadBytes(ChecksumLengthBytes);
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write((uint) CurrencyNetwork);
            writer.Write(Command.Encode());
            writer.Write(PayloadLength);
            writer.Write(Checksum);
        }
    }
}