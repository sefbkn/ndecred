namespace NDecred.TxScript
{
    public class ScriptOptions
    {
	    // Conditionally enabled.
        public bool EnableSha256 { get; set; }
	    
        public int MaxStackSize => 1024;
        public int MaxScriptSize => 16384;
	    public ushort DefaultScriptVersion => 0;
	    
	    // These fields defined in 
	    // https://github.com/decred/dcrd/blob/master/txscript/script.go
        public int MaxScriptElementSize => 2048;
	    public int MaxOperationsPerScript => 255;
	    public int MaxPublicKeysPerMultisig => 20;
    }
}