using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDecred.Common;

namespace NDecred.Wire
{
    public class MsgAddr : Message
    {
        public const int MaxAddrPerMsg = 1000;
        public NetworkAddress[] Addresses { get; set; }
        
        public override void Decode(BinaryReader reader)
        {
            var count = reader.ReadVariableLengthInteger();
            if(count > MaxAddrPerMsg)
                throw new Exception($"Too many addresses for message {count}.  Maximum allowed is {MaxAddrPerMsg}");

            var addresses = Enumerable.Range(1, (int) count)
                .Select(c => new NetworkAddress(true))
                .ToArray();

            foreach (var address in addresses)
                address.Decode(reader);
            
            Addresses = addresses;
        }

        public override void Encode(BinaryWriter writer)
        {
            writer.WriteVariableLengthInteger((ulong)Addresses.Length);
            foreach (var address in Addresses)
                address.Encode(writer);
        }

        public override MsgCommand Command => MsgCommand.Addr;
    }
}