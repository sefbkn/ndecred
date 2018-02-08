using System;
using System.IO;
using System.Linq;

namespace NDecred.Wire
{
    public class MsgNotImplemented : Message
    {
        public override void Decode(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Encode(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override MsgCommand Command => throw new NotImplementedException();
    }
    
    public class MsgCommand : NetworkEncodable
    {
        public const int CommandSizeBytes = 12;

        public static readonly MsgCommand Addr = new MsgCommand("addr", () => new MsgAddr());
        public static readonly MsgCommand Block = new MsgCommand("block", () => new MsgBlock());
        public static readonly MsgCommand GetAddr = new MsgCommand("getaddr", () => new MsgGetAddr());
        public static readonly MsgCommand GetBlocks = new MsgCommand("getblocks", () => new MsgGetBlocks());
        public static readonly MsgCommand GetHeaders = new MsgCommand("getheaders", () => new MsgGetHeaders());
        public static readonly MsgCommand Ping = new MsgCommand("ping", () => new MsgPing());
        public static readonly MsgCommand Pong = new MsgCommand("pong", () => new MsgPong());
        public static readonly MsgCommand Reject = new MsgCommand("reject", () => new MsgReject());
        public static readonly MsgCommand Tx = new MsgCommand("tx", () => new MsgTx());
        public static readonly MsgCommand Version = new MsgCommand("version", () => new MsgVersion());
        public static readonly MsgCommand VerAck = new MsgCommand("verack", () => new MsgVerAck());

        public static readonly MsgCommand Inv = new MsgCommand("inv", () => new MsgNotImplemented());
        public static readonly MsgCommand GetData = new MsgCommand("getdata", () => new MsgNotImplemented());
        public static readonly MsgCommand NotFound = new MsgCommand("notfound", () => new MsgNotImplemented());
        public static readonly MsgCommand Headers = new MsgCommand("headers", () => new MsgNotImplemented());
        public static readonly MsgCommand Alert = new MsgCommand("alert", () => new MsgNotImplemented());
        public static readonly MsgCommand MemPool = new MsgCommand("mempool", () => new MsgNotImplemented());
        public static readonly MsgCommand MiningState = new MsgCommand("miningstate", () => new MsgNotImplemented());
        public static readonly MsgCommand GetMiningState = new MsgCommand("getminings", () => new MsgNotImplemented());
        public static readonly MsgCommand FilterAdd = new MsgCommand("filteradd", () => new MsgNotImplemented());
        public static readonly MsgCommand FilterClear = new MsgCommand("filterclear", () => new MsgNotImplemented());
        public static readonly MsgCommand FilterLoad = new MsgCommand("filterload", () => new MsgNotImplemented());
        public static readonly MsgCommand MerkleBlock = new MsgCommand("merkleblock", () => new MsgNotImplemented());
        public static readonly MsgCommand SendHeaders = new MsgCommand("sendheaders", () => new MsgNotImplemented());
        public static readonly MsgCommand FeeFilter = new MsgCommand("feefilter", () => new MsgNotImplemented());

        private Func<Message> _messageFactory;

        public MsgCommand() { }
        private MsgCommand(string name, Func<Message> messageFactory)
        {
            _messageFactory = messageFactory;
            Name = name;
        }

        public static MsgCommand[] All => new[]
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

        public string Name { get; private set; }

        public Message CreateMessage()
        {
            return _messageFactory();
        }

        public static MsgCommand Find(string commandName)
        {
            foreach (var cmd in All)
            {
                if (cmd.Name != commandName) continue;
                return cmd;
            }

            throw new WireException($"Unrecogized command {commandName}");
        }
        
        public override void Decode(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(CommandSizeBytes);
            
            // The name should only consist of alphanumeric characters.
            var chars = bytes
                .Select(b => (char) b)
                .Where(char.IsLetterOrDigit)
                .ToArray();

            var cmd = Find(new string(chars));
            this.Name = cmd.Name;
            this._messageFactory = cmd._messageFactory;
        }

        public override void Encode(BinaryWriter writer)
        {
            var bytesOut = new byte[CommandSizeBytes];

            if (Name.Length > bytesOut.Length)
                throw new WireException(
                    $"Command name is larger that the maximum allowed size {CommandSizeBytes} bytes");

            for (var i = 0; i < Name.Length; i++)
                bytesOut[i] = (byte) Name[i];

            writer.Write(bytesOut);
        }
        
        public static bool operator ==(MsgCommand a, MsgCommand b)
        {
            return a?.Equals(b) ?? false;
        }

        public static bool operator !=(MsgCommand a, MsgCommand b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MsgCommand cmd)) return false;
            return Name == cmd.Name;
        }

        protected bool Equals(MsgCommand other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}