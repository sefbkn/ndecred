using System;

namespace NDecred.Core.Blockchain
{
    public class Block
    {
        public BlockHeader Header { get; set; }
        public MsgTx[] Transactions { get; set; }
        public MsgTx[] StakeTransactions { get; set; }

        public byte[] BlockHash()
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}