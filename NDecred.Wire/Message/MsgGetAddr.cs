using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDecred.Common;

namespace NDecred.Wire
{
    public class MsgGetAddr : Message
    {        
        public override void Decode(BinaryReader reader)
        {
        }

        public override void Encode(BinaryWriter writer)
        {
        }

        public override MsgCommand Command => MsgCommand.GetAddr;
    }
}