using System;
using System.IO;
using System.Linq;

namespace NDecred.Core.Blockchain
{
    public class MsgBlock
    {
        public BlockHeader Header { get; set; }
        public MsgTx[] Transactions { get; set; }
        public MsgTx[] StakeTransactions { get; set; }

        public byte[] BlockHash()
        {
            return Header.GetHash();
        }

        public byte[][] TxHashes()
        {
            return Transactions.Select(t => t.GetHash()).ToArray();
        }

        public byte[][] STxHashes()
        {
            return StakeTransactions.Select(t => t.GetHash()).ToArray();
        }

        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(Header.Serialize());
                
                bw.WriteVariableLengthInteger((ulong) Transactions.Length);
                foreach(var tx in Transactions)
                    bw.Write(tx.Serialize());

                bw.WriteVariableLengthInteger((ulong) StakeTransactions.Length);
                foreach(var tx in StakeTransactions)
                    bw.Write(tx.Serialize());

                return ms.ToArray();
            }
        }
    }
}