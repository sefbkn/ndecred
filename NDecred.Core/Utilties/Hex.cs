using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.Core
{
    public static class Hex
    {
        public static byte[] ToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
        }

        public static string FromByteArray(IEnumerable<byte> bytes)
        {
            return BitConverter.ToString(bytes.ToArray()).Replace("-", string.Empty).ToLower();
        }
    }
}