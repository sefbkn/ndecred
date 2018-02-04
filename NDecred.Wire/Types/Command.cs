using System;
using System.IO;
using System.Linq;

namespace NDecred.Wire.Types
{
    public class Command : NetworkEncodable
    {
        public static readonly int CommandSizeBytes = 12;

        public static readonly Command Version = new Command("version");
        public static readonly Command VerAck = new Command("verack");
        public static readonly Command GetAddr = new Command("getaddr");
        public static readonly Command Addr = new Command("addr");
        public static readonly Command GetBlocks = new Command("getblocks");
        public static readonly Command Inv = new Command("inv");
        public static readonly Command GetData = new Command("getdata");
        public static readonly Command NotFound = new Command("notfound");
        public static readonly Command Block = new Command("block");
        public static readonly Command Tx = new Command("tx");
        public static readonly Command GetHeaders = new Command("getheaders");
        public static readonly Command Headers = new Command("headers");
        public static readonly Command Ping = new Command("ping");
        public static readonly Command Pong = new Command("pong");
        public static readonly Command Alert = new Command("alert");
        public static readonly Command MemPool = new Command("mempool");
        public static readonly Command MiningState = new Command("miningstate");
        public static readonly Command GetMiningState = new Command("getminings");
        public static readonly Command FilterAdd = new Command("filteradd");
        public static readonly Command FilterClear = new Command("filterclear");
        public static readonly Command FilterLoad = new Command("filterload");
        public static readonly Command MerkleBlock = new Command("merkleblock");
        public static readonly Command Reject = new Command("reject");
        public static readonly Command SendHeaders = new Command("sendheaders");
        public static readonly Command FeeFilter = new Command("feefilter");

        public Command(){}
        private Command(string name)
        {
            Name = name;
        }

        public static Command[] All => new[]
        {
            Version,
            VerAck,
            GetAddr,
            Addr,
            GetBlocks,
            Inv,
            GetData,
            NotFound,
            Block,
            Tx,
            GetHeaders,
            Headers,
            Ping,
            Pong,
            Alert,
            MemPool,
            MiningState,
            GetMiningState,
            FilterAdd,
            FilterClear,
            FilterLoad,
            MerkleBlock,
            Reject,
            SendHeaders,
            FeeFilter
        };

        public string Name { get; set; }

        public override void Decode(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(CommandSizeBytes);
            
            // The name should only consist of alphanumeric characters.
            var chars = bytes
                .Select(b => (char) b)
                .Where(char.IsLetterOrDigit)
                .ToArray();

            var commandName = new string(chars);
            foreach (var cmd in All)
                if (cmd.Name == commandName)
                    Name = cmd.Name;

            throw new Exception($"Unrecogized command {commandName}");
        }

        public override void Encode(BinaryWriter writer)
        {
            var bytesOut = new byte[CommandSizeBytes];

            if (Name.Length > bytesOut.Length)
                throw new Exception(
                    $"Command name is larger that the maximum allowed size {CommandSizeBytes} bytes");

            for (var i = 0; i < Name.Length; i++)
                bytesOut[i] = (byte) Name[i];

            writer.Write(bytesOut);
        }
    }
}