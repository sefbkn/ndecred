using System;
using System.Collections.Generic;
using System.Linq;
using NDecred.Common;

namespace NDecred.Blockchain
{   
    public class MerkleTree
    {
        /// <summary>
        /// Calculates the merkle root given an array of hashes of the leaf nodes.
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public byte[] GetRoot(byte[][] hashes)
        {
            if (hashes == null || hashes.Length == 0)
                return new byte[32];
            
            var input = ExtendCollectionSize(hashes);
            var queue = new Queue<byte[]>(input);

            while (queue.Count > 1)
            {
                var a = queue.Dequeue();
                var b = queue.Dequeue() ?? a;
                
                // If both elements are null, return null.
                // Otherwise, return the hash of both concatenanted
                var parent = a == null && b == null ? null : HashUtil.Blake256(a, b);
                queue.Enqueue(parent);
            }

            // The last element is the merkle root.
            return queue.Dequeue();
        }

        // Expand the size of the collection to the next power of two.
        // Doing this ensures the tree is balanced when building.
        private IEnumerable<byte[]> ExtendCollectionSize(byte[][] leafHashes)
        {
            var targetSize = RoundUpToNextPowerOfTwo(leafHashes.Length);
            return leafHashes.Concat(Enumerable.Repeat((byte[])null, targetSize)).Take(targetSize);
        }

        /// <summary>
        /// Returns the smallest power of two larger than the given number
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private int RoundUpToNextPowerOfTwo(int n)
        {
            // already power of 2, only 1 bit set.
            if ((n & (n - 1)) == 0) return n;
            return 1 << (int) Math.Log(n, 2) + 1;
        }
    }
}