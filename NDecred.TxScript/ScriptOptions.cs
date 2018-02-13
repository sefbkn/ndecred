namespace NDecred.TxScript
{
    public class ScriptOptions
    {
        public bool ValidatePayToScriptHashFull { get; set; }
        public bool StrictMultiSig { get; set; }
	    
        public bool DiscourageUpgradableNops { get; set; }
        public bool CheckLockTimeVerify { get; set; }
        public bool CheckSequenceVerify { get; set; }
        public bool VerifyCleanStack { get; set; }
        public bool VerifyDerSignatures { get; set; }
        
        public bool VerifyLowS { get; set; }
        public bool VerifyMinimalData { get; set; }
        public bool VerifySigPushOnly { get; set; }
        public bool VerifyStrictEncoding { get; set; }
        
        // Enabled in a previous vote
        public bool VerifySha256 { get; set; }

        public int MaxStackSize => 1024;
        public int MaxScriptSize => 16384;
        public ushort DefaultScriptVersion => 0;
    }
}