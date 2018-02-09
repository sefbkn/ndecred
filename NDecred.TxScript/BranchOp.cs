namespace NDecred.TxScript
{
    /// <summary>
    /// Flag that determines whether the current instruction should run.
    /// 
    /// Used to enabled / disable blocks of code when executing branches.
    /// </summary>
    public enum BranchOp
    {
        False = 0,
        True = 1,
        Skip = 2
    }
}