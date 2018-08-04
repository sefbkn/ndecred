using System.IO;
using System.Linq;
using NDecred.Common;

namespace NDecred.Wire
{
    public class MsgGetHeaders : Message
    {
        public uint MsgProtocolVersion { get; set; }
        public byte[][] BlockLocatorHashes { get; set; }
        public byte[] HashStop { get; set; }

        public MsgGetHeaders()
        {
            BlockLocatorHashes = new byte[0][];
	        HashStop = new byte[0];
        }
        
        public override void Decode(BinaryReader reader)
        {
	        MsgProtocolVersion = reader.ReadUInt32();

	        var count = reader.ReadVariableLengthInteger();
	        if (count > MsgGetBlocks.MaxBlockLocatorHashesPerMsg)
		        throw new WireException($"Block locator {count} is greater than max allowed {MsgGetBlocks.MaxBlockLocatorHashesPerMsg}");

	        var hashes = new byte[count][];
	        for (var i = 0; i < hashes.Length; i++)
		        hashes[i] = reader.ReadBytes(32);
	        BlockLocatorHashes = hashes;

	        HashStop = reader.ReadBytes(32);
        }

        public override void Encode(BinaryWriter writer)
        {
	        writer.Write(ProtocolVersion);
	        writer.WriteVariableLengthInteger(BlockLocatorHashes.Length);
	        foreach(var locatorHash in BlockLocatorHashes)
		        writer.Write(locatorHash);
	        writer.Write(HashStop);
        }

        public override MsgCommand Command => MsgCommand.GetHeaders;
    }
}