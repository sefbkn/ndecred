﻿using System;

namespace NDecred.Wire
{
    public enum TxTree : byte
    {
        TxTreeUnknown = 0xff, // -1
        TxTreeRegular = 0,
        TxTreeStake = 1
    }

    public class OutPoint
    {
        public OutPoint()
        {
            Hash = new byte[32];
        }

        public OutPoint(byte[] hash, uint index, TxTree tree)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash), "OutPoint hash parameter must not be null");
            if (hash.Length != 32)
                throw new ArgumentException("OutPoint hash parameter length must equal 32");

            Hash = hash;
            Index = index;
            Tree = (byte) tree;
        }

        /// <summary>
        ///     The hash of ?
        /// </summary>
        public byte[] Hash { get; set; }

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