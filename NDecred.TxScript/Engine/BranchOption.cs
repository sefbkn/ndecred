namespace NDecred.TxScript
{
    /// <summary>
    /// Used to toggle execution of instructions within conditional blocks in the ScriptEngine
    /// </summary>
    public enum BranchOption
    {
        False = 0,
        True,
        Skip
    }
}