using System;
using System.Collections.Generic;
using System.Linq;
using NDecred.Common;
using NDecred.Core;
using Xunit;

namespace NDecred.Blockchain.Tests
{
    public class MerkleTreeTests
    {
        private readonly MerkleTree _subject;
        private readonly byte[][] _hashes;

        public MerkleTreeTests()
        {
            _subject = new MerkleTree();
            _hashes = new []
            {
                HashUtil.Blake256(new byte[]{0x01}),
                HashUtil.Blake256(new byte[]{0x02}),
                HashUtil.Blake256(new byte[]{0x03}),
                HashUtil.Blake256(new byte[]{0x04}),
                HashUtil.Blake256(new byte[]{0x05}),
            };
        }

        [Fact]
        public void GetRoot_GivenNoHashes_ReturnsAllZeroBytesMerkleRoot()
        {
            var expected = new byte[32];
            var result = _subject.GetRoot(new byte[0][]); 
            Assert.Equal(expected, (IEnumerable<byte>) result);
        }

        [Fact]
        public void GetRoot_GivenSingleHash_ReturnsSameHashAsRoot()
        {
            var bytes = new byte[32];
            var result = _subject.GetRoot(new[] { bytes });   
            Assert.Equal(bytes, (IEnumerable<byte>) result);
        }
        
        [Fact]
        public void GetRoot_GivenTwoHashes_ReturnsHashOfTwoElements()
        {            
            var hashA = _hashes[0];
            var hashB = _hashes[1];
            
            var result = _subject.GetRoot(new[]{ hashA, hashB });   

            var expected = HashUtil.Blake256(hashA.Concat(hashB).ToArray());
            Assert.Equal(expected, (IEnumerable<byte>) result);
        }
        
        [Fact]
        public void GetRoot_GivenThreeHashes_ReturnsHashOfThreeElements()
        {            
            var hashA = _hashes[0];
            var hashB = _hashes[1];
            var hashC = _hashes[3];
            
            var result = _subject.GetRoot(new[]{ hashA, hashB, hashC });   

            // Since the number of elements is not balanced, we should make sure that
            // the logic hashes C with itself as the tree is build.
            
            var hashAB = HashUtil.Blake256(hashA, hashB);
            var hashCC = HashUtil.Blake256(hashC, hashC);
            var expected = HashUtil.Blake256(hashAB, hashCC);
            
            Assert.Equal(expected, (IEnumerable<byte>) result);
        }
        
        [Fact]
        public void GetRoot_GivenFiveHashes_ReturnsHashOfFiveElements()
        {
            var hashA = _hashes[0];
            var hashB = _hashes[1];
            var hashC = _hashes[2];
            var hashD = _hashes[3];
            var hashE = _hashes[4];
            
            var result = _subject.GetRoot(new[]{ hashA, hashB, hashC, hashD, hashE });
            var expected = Hex.ToByteArray("c2ae6cb5d77963061d116a4f070de42dbfd5a0f9fc940cde21335a5eab95d92f");

            Assert.Equal(expected, (IEnumerable<byte>) result);
        }
    }
}