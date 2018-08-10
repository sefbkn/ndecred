using System;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.Common
{
    public static class Hex
    {
        /// <summary>
        ///     Converts a string of hexadecimal characters to a byte[]
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        /// <summary>
        ///     Converts a byte[] to a string of lowercase hexadecimal characters
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FromByteArray(IEnumerable<byte> bytes)
        {
            return BitConverter.ToString(bytes.ToArray()).Replace("-", string.Empty).ToLower();
        }
    }
}