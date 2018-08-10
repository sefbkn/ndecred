using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDecred.Common;

namespace NDecred.Wire
{
    public class MsgBlock : Message
    {
        public MsgBlock()
        {
            Header = new BlockHeader();
        }

        public BlockHeader Header { get; set; }
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
            Header.Decode(reader);

            var txCount = reader.ReadVariableLengthInteger();
            Transactions = Enumerable.Repeat(new MsgTx(), (int) txCount).ToArray();
            for (long i = 0; i < txCount; i++)
                Transactions[i].Decode(reader);
            
            var stxCount = reader.ReadVariableLengthInteger();
            StakeTransactions = Enumerable.Repeat(new MsgTx(), (int) stxCount).ToArray();
            for (long i = 0; i < stxCount; i++)
                StakeTransactions[i].Decode(reader);
        }

        public override void Encode(BinaryWriter writer)
        {
            Header.Encode(writer);

            writer.WriteVariableLengthInteger(Transactions.Length);
            foreach (var tx in Transactions)
                tx.Encode(writer);

            writer.WriteVariableLengthInteger(StakeTransactions.Length);
            foreach (var tx in StakeTransactions)
                tx.Encode(writer);
        }
        
        public override MsgCommand Command => MsgCommand.Block;
    }
}