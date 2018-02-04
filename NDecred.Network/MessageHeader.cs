using System;
using System.IO;
using System.Linq;
using NDecred.Common;
using NDecred.Core;

namespace NDecred.Network
{
    /// <summary>
    /// Message header structure
    /// 
    /// 
    /// 
    /// </summary>
    public class MessageHeader : NetworkMessage
    {
        public static readonly int ChecksumLengthBytes = 4;
        
        public MessageHeader(){ }
        public MessageHeader(CurrencyNet currencyNet, Command command, byte[] payload)
        {
            CurrencyNetwork = currencyNet;
            Command = command;
            PayloadLength = payload.Length;
            Checksum = Hash.BLAKE256(payload).Take(ChecksumLengthBytes).ToArray();
        }

        public CurrencyNet CurrencyNetwork { get; set; }
        public Command Command { get; set; }
        public int PayloadLength { get; set; }
        public byte[] Checksum { get; set;  }

        public override void Decode(BinaryReader reader)
        {
            CurrencyNetwork = (CurrencyNet) reader.ReadUInt32();
            
            var commandBytes = reader.ReadBytes(Command.CommandSizeBytes);
            Command = Command.Decode(commandBytes);

            PayloadLength = reader.ReadInt32();
            Checksum = reader.ReadBytes(ChecksumLengthBytes);
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write((uint)CurrencyNetwork);
            writer.Write(Command.Encode());
            writer.Write(PayloadLength);
            writer.Write(Checksum);
        }
    }
}