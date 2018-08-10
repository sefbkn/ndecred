using System.IO;
using NDecred.Common;

namespace NDecred.Wire
{
	public class MsgReject : Message
    {
	    public long MaxMessagePayload = (1024 * 1024 * 32);
	    
	    // The message that was rejected
	    public string RejectedCommand { get; set; }
	    
	    // The code representing the reason for rejection
	    public RejectCode Code { get; set; }
	    
	    // Human-friendly reason for rejection
	    public string Reason { get; set; }
	    
	    // Hash of block or transaction rejected. Only for MsgBlock + MsgTx rejection reasons.
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
	        writer.WriteVariableLengthString(RejectedCommand);
	        writer.Write((byte) Code);
	        writer.WriteVariableLengthString(Reason);
	        
	        var msgCommand = MsgCommand.Find(RejectedCommand);
	        if (msgCommand == MsgCommand.Block || msgCommand == MsgCommand.Tx)
	        {
		        writer.Write(Hash);
	        }
        }
	    
	    public override MsgCommand Command => MsgCommand.Reject;
    }
}