using System;
using System.IO;
using NDecred.Common;

namespace NDecred.Wire
{
    // MsgVersion implements the Message interface and represents a decred version
    // message.  It is used for a peer to advertise itself as soon as an outbound
    // connection is made.  The remote peer then uses this information along with
    // its own to negotiate.  The remote peer must then respond with a version
    // message of its own containing the negotiated values followed by a verack
    // message (MsgVerAck).  This exchange must take place before any further
    // communication is allowed to proceed.
    public class MsgVersion : NetworkEncodable
    {
        public const int MaxUserAgentLen = 256;
        public const string DefaultUserAgent = "/ndecred:0.1.0/";

        public MsgVersion()
        {
            UserAgent = DefaultUserAgent;
            AddrMe = new NetworkAddress();
            AddrYou = new NetworkAddress();
        }

        // Version of the protocol the node is using.
        public int ProtocolVersion { get; set; }

        // Bitfield which identifies the enabled services.
        public ServiceFlag Services { get; set; }

        // Time the message was generated.  This is encoded as an int64 on the wire.
        public DateTime Timestamp { get; set; }

        // Address of the remote peer.
        public NetworkAddress AddrYou { get; set; }

        // Address of the local peer.
        public NetworkAddress AddrMe { get; set; }

        // Unique value associated with message that is used to detect self
        // connections.
        public ulong Nonce { get; set; }

        // The user agent that generated messsage.  This is a encoded as a varString
        // on the wire.  This has a max length of MaxUserAgentLen.
        public string UserAgent { get; set; }

        // Last block seen by the generator of the version message.
        public int LastBlock { get; set; }

        // Don't announce transactions to peer.
        public bool DisableRelayTx { get; set; }

        public override void Decode(BinaryReader reader)
        {
            ProtocolVersion = reader.ReadInt32();
            Services = (ServiceFlag) reader.ReadUInt64();
            Timestamp = DateTimeExtensions.FromUnixTime(reader.ReadInt64());

            AddrYou = new NetworkAddress();
            AddrYou.Decode(reader);

            AddrMe = new NetworkAddress();
            AddrMe.Decode(reader);

            Nonce = reader.ReadUInt64();
            UserAgent = reader.ReadVariableLengthString(MaxUserAgentLen);
            LastBlock = reader.ReadInt32();
            DisableRelayTx = !reader.ReadBoolean();
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.Write(ProtocolVersion);
            writer.Write((ulong) Services);
            writer.Write(Timestamp.ToUnixTime());

            AddrYou.Encode(writer);
            AddrMe.Encode(writer);

            writer.Write(Nonce);
            writer.WriteVariableLengthString(UserAgent);
            writer.Write(LastBlock);
            writer.Write(!DisableRelayTx);
        }
    }
}