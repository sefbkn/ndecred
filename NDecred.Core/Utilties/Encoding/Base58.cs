using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NDecred.Core
{
    public class Base58 : BaseEncoding
    {
        private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        public override string Encode(byte[] payload)
        {
            var bigint = payload.Aggregate(new BigInteger(), (current, t) => current * 256 + t);

            var stringBuilder = new StringBuilder();
            while (bigint > 0)
            {
                var remainder = (int) (bigint % Alphabet.Length);
                bigint /= Alphabet.Length;
                stringBuilder.Insert(0, Alphabet[remainder]);
            }

            for (var i = 0; i < payload.Length && payload[i] == 0; i++)
                stringBuilder.Insert(0, "1");

            return stringBuilder.ToString();
        }

        public override byte[] Decode(string value)
        {
            BigInteger intData = 0;
            for (var i = 0; i < value.Length; i++)
            {
                var digit = Alphabet.IndexOf(value[i]); //Slow
                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", value[i],
                        i));
                intData = intData * Alphabet.Length + digit;
            }

            var leadingZeroCount = value.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte) 0, leadingZeroCount);
            var bytesWithoutLeadingZeros = intData.ToByteArray().Reverse().SkipWhile(b => b == 0);
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }
    }
}