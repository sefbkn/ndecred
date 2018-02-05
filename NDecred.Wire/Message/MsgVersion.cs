using System;
using System.IO;
using NDecred.Common;

namespace NDecred.Wire
{
    public class MsgVersion : Message
    {
        public const int MaxUserAgentLen = 256;
        public const string DefaultUserAgent = "/ndecred:0.0.0/";

        public MsgVersion()
        {
            UserAgent = DefaultUserAgent;
            AddrMe = new NetworkAddress(false);
            AddrYou = new NetworkAddress(false);
        }

        public int NodeProtocolVersion { get; set; }
        public ServiceFlag Services { get; set; }
        public DateTime Timestamp { get; set; }
        public NetworkAddress AddrYou { get; set; }
        public NetworkAddress AddrMe { get; set; }
        public ulong Nonce { get; set; }
        public string UserAgent { get; set; }
        public int LastBlock { get; set; }
        public bool DisableRelayTx { get; set; }

        public override void Decode(BinaryReader reader)
        {
            NodeProtocolVersion = reader.ReadInt32();
            Services = (ServiceFlag) reader.ReadUInt64();
            Timestamp = DateTimeExtensions.FromUnixTime(reader.ReadInt64());

            AddrYou = new NetworkAddress(false);
            AddrYou.Decode(reader);

            AddrMe = new NetworkAddress(false);
            AddrMe.Decode(reader);

            Nonce = reader.ReadUInt64();
            UserAgent = reader.ReadVariableLengthString(MaxUserAgentLen);
            LastBlock = reader.ReadInt32();
            DisableRelayTx = !reader.ReadBoolean();
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(NodeProtocolVersion);
            writer.Write((ulong) Services);
            writer.Write(Timestamp.ToUnixTime());

            AddrYou.Encode(writer);
            AddrMe.Encode(writer);

            writer.Write(Nonce);
            writer.WriteVariableLengthString(UserAgent);
            writer.Write(LastBlock);
            writer.Write(!DisableRelayTx);
        }
        
        public override MsgCommand Command => MsgCommand.Version;
    }
}