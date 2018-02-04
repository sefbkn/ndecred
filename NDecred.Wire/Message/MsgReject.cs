using System.IO;
using NDecred.Common;

namespace NDecred.Wire
{
    // These constants define the various supported reject codes.
    public enum RejectCode
    {
        RejectMalformed       = 0x01,
        RejectInvalid         = 0x10,
        RejectObsolete        = 0x11,
        RejectDuplicate       = 0x12,
        RejectNonstandard     = 0x40,
        RejectDust            = 0x41,
        RejectInsufficientFee = 0x42,
        RejectCheckpoint      = 0x43
    }
    
    /*
     * type MsgReject struct {
	// Cmd is the command for the message which was rejected such as
	// as CmdBlock or CmdTx.  This can be obtained from the Command function
	// of a Message.
	Cmd string

	// RejectCode is a code indicating why the command was rejected.  It
	// is encoded as a uint8 on the wire.
	Code RejectCode

	// Reason is a human-readable string with specific details (over and
	// above the reject code) about why the command was rejected.
	Reason string

	// Hash identifies a specific block or transaction that was rejected
	// and therefore only applies the MsgBlock and MsgTx messages.
	Hash chainhash.Hash
}

     */

    public class MsgReject : NetworkEncodable
    {
	    public ulong MaxMessagePayload = (1024 * 1024 * 32);
	    
	    public string Command { get; set; }
	    public RejectCode Code { get; set; }
	    public string Reason { get; set; }
	    public byte[] Hash { get; set; }

	    public MsgReject()
	    {
		    Hash = new byte[32];
	    }
	    
        public override void Decode(BinaryReader reader)
        {
	        Command = reader.ReadVariableLengthString(MaxMessagePayload);
	        Code = (RejectCode) reader.ReadByte();
	        Reason = reader.ReadVariableLengthString(MaxMessagePayload);

	        var msgCommand = MsgCommand.Find(Command);

	        if (msgCommand == MsgCommand.Block || msgCommand == MsgCommand.Tx)
	        {
		        Hash = reader.ReadBytes(32);
	        }
        }

        public override void Encode(BinaryWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}