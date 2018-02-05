using System.IO;
using NDecred.Common;

namespace NDecred.Wire
{
    // These constants define the various supported reject codes.

	public class MsgReject : Message
    {
	    public ulong MaxMessagePayload = (1024 * 1024 * 32);
	    
	    // The message that was rejected
	    public string RejectedCommand { get; set; }
	    public RejectCode Code { get; set; }
	    public string Reason { get; set; }
	    public byte[] Hash { get; set; }

	    public MsgReject()
	    {
		    Hash = new byte[32];
	    }
	    
        public override void Decode(BinaryReader reader)
        {
	        RejectedCommand = reader.ReadVariableLengthString(MaxMessagePayload);
	        Code = (RejectCode) reader.ReadByte();
	        Reason = reader.ReadVariableLengthString(MaxMessagePayload);

	        var msgCommand = MsgCommand.Find(RejectedCommand);

	        if (msgCommand == MsgCommand.Block || msgCommand == MsgCommand.Tx)
	        {
		        Hash = reader.ReadBytes(32);
	        }
        }

        public override void Encode(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
	    
	    public override MsgCommand Command => MsgCommand.Reject;
    }
}