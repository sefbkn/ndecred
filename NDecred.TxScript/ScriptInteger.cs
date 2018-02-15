using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NDecred.TxScript
{
    /// <summary>
    /// Represents an integer that operated on by a script.
    /// </summary>
    public struct ScriptInteger
    {
        public const int MathOpcodeMaxLength = 4;
        
        private const int MaxInt32 = int.MaxValue;
        private const int MinInt32 = int.MinValue + 1;        
        private static readonly byte[] Zero = new byte[0];
        
        private long Value { get; }

        public ScriptInteger(long value)
        {
            Value = value;
        }

        public ScriptInteger(byte[] bytes, bool assertMinimalEncoding, int maxLength)
        {
            if (bytes.Length > maxLength)
                throw new ScriptIntegerEncodingException(bytes, 
                    $"Script integer byte length expected to be <= {maxLength}.  Actual value {bytes.Length}");
            
            if(bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if(assertMinimalEncoding)
                AssertMinimalEncoding(bytes);

            Value = bytes.Length == 0 ? 0 : FromBytes(bytes);
        }
        
        private static void AssertMinimalEncoding(byte[] bytes)
        {
            var length = bytes.Length;
            
            if (bytes.Length == 0)
                return;
            
            // Interpret single-byte values as literals,
            // with the exception of 0x80.  That value should
            // be extended with a sign byte.

            if ((bytes[length - 1] & 0x7f) != 0) 
                return;
            if (length == 1)
                throw new ScriptIntegerEncodingException(bytes, "Script integer bytes not minimally encoded.  Should be extended with a sign byte.");
            if((bytes[length - 2] & 0x80) == 0)
                throw new ScriptIntegerEncodingException(bytes, "Script integer bytes not minimally encoded. Can be compressed further by removing the sign byte.");
        }

        private static long FromBytes(byte[] bytes)
        {            
            long result = 0;
            
            // If the leading bit is set, the number is negative.
            var isNegative = (bytes.Last() & 0x80) != 0;
            
            // Extract bytes and store in long value (little -> big endian)
            for (byte i = 0; i < bytes.Length; i++)
                result |= (long) bytes[i] << i * 8;
            
            if (!isNegative) return result;
            
            // If the original value was negative,
            // unset the negative-flag bit from the encoded value.
            // Negating the long value will set the high bit for us.
            result &= ~((long) 0x80 << 8 * (bytes.Length - 1));
            return -result;
        }

        public byte[] ToBytes()
        {
            if(Value == 0)
                return Zero;

            var n = Value;
            var isNegative = n < 0;
            if (isNegative)
            {
                n = -n;
            }

            // Shift bytes out 1 at a time.
            var byteList = new List<byte>(9);
            do
            {
                byteList.Add((byte)(n&0xff));
                n >>= 8;
            } while (n > 0);

            if ((byteList.Last() & 0x80) != 0)
            {
                byteList.Add((byte)(isNegative ? 0x80 : 0x00));
            }
            
            else if(isNegative)
            {
                byteList[byteList.Count - 1] |= 0x80;
            }

            return byteList.ToArray();
        }

        public static explicit operator int(ScriptInteger d)
        {
            if (d.Value > MaxInt32)
                return MaxInt32;
            if (d.Value < MinInt32)
                return MinInt32;

            return (int) d.Value;
        }
    }
}