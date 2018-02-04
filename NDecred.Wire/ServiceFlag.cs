using System;

namespace NDecred.Wire
{
    [Flags]
    public enum ServiceFlag : ulong
    {
        // SFNodeNetwork is a flag used to indicate a peer is a full node.
        SFNodeNetwork = 1,

        // SFNodeBloom is a flag used to indiciate a peer supports bloom
        // filtering.
        SFNodeBloom
    }
}