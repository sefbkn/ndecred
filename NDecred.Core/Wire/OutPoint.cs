using System;
using System.Runtime.Serialization;

namespace NDecred.Core
{
    // OutPoint defines a bitcoin data type that is used to track previous
    // transaction outputs.
    public class OutPoint
    {
        public OutPoint(byte[] hash, uint index, byte tree)
        {
            if(hash == null)
                throw new ArgumentNullException(nameof(hash), "OutPoint hash parameter must not be null");
            if(hash.Length != 32)
                throw new ArgumentException("OutPoint hash parameter length must equal 32");
            
            Hash = hash;
            Index = index;
            Tree = tree;
        }

        /// <summary>
        ///     The hash of ?
        /// </summary>
        public byte[] Hash { get; }

        /// <summary>
        ///     The index of ? in ?
        /// </summary>
        public uint Index { get; }

        /// <summary>
        ///     Tree?
        /// </summary>
        public byte Tree { get; }
    }
}