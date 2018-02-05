namespace NDecred.Wire
{
    public enum RejectCode
    {
        Malformed       = 0x01,
        Invalid         = 0x10,
        Obsolete        = 0x11,
        Duplicate       = 0x12,
        Nonstandard     = 0x40,
        Dust            = 0x41,
        InsufficientFee = 0x42,
        Checkpoint      = 0x43
    }
}