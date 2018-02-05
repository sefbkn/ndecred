using System;

namespace NDecred.Wire
{
    [Flags]
    public enum ServiceFlag : ulong
    {
        // Full node
        NodeNetwork = 1,

        // Node supports bloom filtering
        NodeBloom
    }
}