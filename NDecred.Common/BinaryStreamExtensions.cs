using System;
using System.IO;
using System.Linq;

namespace NDecred.Common
{
    public static class BinaryStreamExtensions
    {
        /// <summary>
        ///     Writes a variable length integer to the underlying stream at the current position.
        ///     https://en.bitcoin.it/wiki/Protocol_documentation#Variable_length_integer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteVariableLengthInteger(this BinaryWriter writer, long value)
        {
            var unsignedValue = (ulong) value;
            var format = (byte) (
                unsignedValue < 0xfd ? 0 : // write byte, no flags
                unsignedValue <= ushort.MaxValue ? 0xfd : // write ushort
                unsignedValue <= uint.MaxValue ? 0xfe : // write uint
                0xff // write ulong
            );

            if (format == 0)
            {
                writer.Write((byte) value);
            }
            else
            {
                writer.Write(format);

                if (format == 0xFF)
                    writer.Write(value);
                else if (format == 0xFE)
                    writer.Write(value);
                else if (format == 0xFD)
                    writer.Write(value);
            }
        }

        /// <summary>
        ///     Reads a variable length integer from the current position of the BinaryReader
        ///     and returns the value as an unsigned 64bit integer.
        ///     https://en.bitcoin.it/wiki/Protocol_documentation#Variable_length_integer
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>A 64 bit number containing the variable integer.</returns>
        /// <exception cref="Exception">
        ///     If the parsed value is smaller than the minimum expected value for the amount of space
        ///     consumed, an exception is thrown.
        /// </exception>
        public static long ReadVariableLengthInteger(this BinaryReader reader)
        {
            long value;
            ulong min = 0;
            var format = reader.ReadByte();

            switch (format)
            {
                case 0xFF:
                    min = (long) uint.MaxValue + 1;
                    value = reader.ReadInt64();
                    break;
                case 0xFE:
                    min = (long) ushort.MaxValue + 1;
                    value = reader.ReadInt32();
                    break;
                case 0xFD:
                    min = 0xFD;
                    value = reader.ReadInt16();
                    break;
                default:
                    value = format;
                    break;
            }

            if (min > (ulong) value)
            {
                throw new Exception(
                    $"Noncanonical varint {value}. " +
                    $"Discriminant 0x{format:X} must encode a value greater than {min}");
            }

            return value;
        }

        public static byte[] ReadVariableLengthBytes(this BinaryReader reader, long maxLength)
        {
            var length = reader.ReadVariableLengthInteger();
            if (length > maxLength) throw new Exception("payload length prefix is longer than max allowed length");
            return reader.ReadBytes((int) length);
        }

        public static string ReadVariableLengthString(this BinaryReader reader, long maxLength)
        {
            var chars = reader.ReadVariableLengthBytes(maxLength)
                .Select(b => (char) b)
                .ToArray();

            return new string(chars);
        }

        public static void WriteVariableLengthBytes(this BinaryWriter writer, byte[] bytes)
        {
            var length = (bytes?.Length ?? 0);
            writer.WriteVariableLengthInteger(length);
            writer.Write(bytes);
        }

        public static void WriteVariableLengthString(this BinaryWriter writer, string str)
        {
            var length = (str?.Length ?? 0);
            writer.WriteVariableLengthInteger(length);
            writer.Write(str);
        }
    }
}
