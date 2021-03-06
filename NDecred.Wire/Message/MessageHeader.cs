﻿using System.IO;
using System.Linq;
using NDecred.Common;

namespace NDecred.Wire
{
    /// <summary>
    /// Represents a header that is used to describe
    /// the payload sent/received over the network.
    /// </summary>
    public class MessageHeader : NetworkEncodable
    {
        public static readonly int ChecksumLengthBytes = 4;

        public MessageHeader()
        {
        }

        public MessageHeader(CurrencyNet currencyNet, MsgCommand command, byte[] payload)
        {
            CurrencyNetwork = currencyNet;
            Command = command;
            PayloadLength = payload.Length;
            Checksum = HashUtil.Blake256(payload).Take(ChecksumLengthBytes).ToArray();
        }

        public CurrencyNet CurrencyNetwork { get; set; }
        public MsgCommand Command { get; set; }
        public int PayloadLength { get; set; }
        public byte[] Checksum { get; set; }

        public override void Decode(BinaryReader reader)
        {
            CurrencyNetwork = (CurrencyNet) reader.ReadUInt32();

            Command = new MsgCommand();
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