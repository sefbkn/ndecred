namespace NDecred.Wire
{
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
}