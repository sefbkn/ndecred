namespace NDecred.Core.Blockchain
{
    // OutPoint defines a bitcoin data type that is used to track previous
    // transaction outputs.
    public struct OutPoint
    {
        public OutPoint(byte[] hash, uint index, byte tree)
        {
            Hash = hash;
            Index = index;
            Tree = tree;
        }

        /// <summary>
        ///     The hash of ?
        /// </summary>
        public byte[] Hash { get; set;  }

        /// <summary>
        ///     The index of ? in ?
        /// </summary>
        public uint Index { get; set; }

        /// <summary>
        ///     Tree?
        /// </summary>
        public byte Tree { get; set; }
    }
}