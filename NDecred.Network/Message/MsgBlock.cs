using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDecred.Common;

namespace NDecred.Network
{
    public class MsgBlock : NetworkEncodable
    {
        public MsgBlock()
        {
            Header = new MsgBlockHeader();
        }

        public MsgBlockHeader Header { get; set; }
        public MsgTx[] Transactions { get; set; }
        public MsgTx[] StakeTransactions { get; set; }

        public byte[] BlockHash()
        {
            return Header.GetHash();
        }

        public IEnumerable<byte[]> TxHashes()
        {
            return Transactions.Select(t => t.GetHash());
        }

        public IEnumerable<byte[]> STxHashes()
        {
            return StakeTransactions.Select(t => t.GetHash());
        }

        public override void Decode(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Encode(BinaryWriter writer)
        {
            Header.Encode(writer);

            writer.WriteVariableLengthInteger((ulong) Transactions.Length);
            foreach (var tx in Transactions)
                tx.Encode(writer);

            writer.WriteVariableLengthInteger((ulong) StakeTransactions.Length);
            foreach (var tx in StakeTransactions)
                tx.Encode(writer);
        }
    }
}