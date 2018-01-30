using System;
using System.Linq;

namespace NDecred.Core
{
    /// <summary>
    ///     Base class for n-bit unsigned integers stored in a byte[]
    ///     TODO: Optimize as needed.
    /// </summary>
    public abstract class uintn : IComparable<uintn>
    {
        private readonly int _numBytes;
        protected readonly byte[] Bytes;

        protected uintn(int numBytes, string hexBytes) : this(numBytes, Hex.ToByteArray(hexBytes))
        {
        }

        protected uintn(int numBytes, byte[] bytes)
        {
            if (numBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(numBytes), "sizeBytes must be > 0");
            if (bytes == null)
                throw new ArgumentException(nameof(bytes), "should not be null.");
            if (bytes.Length != numBytes)
                throw new ArgumentException(nameof(bytes), $"should contain {numBytes} bytes.");

            _numBytes = numBytes;
            Bytes = new byte[numBytes];

            Buffer.BlockCopy(bytes, 0, Bytes, 0, _numBytes);
        }

        public byte[] RawBytes => Bytes.ToArray();

        /// <summary>
        ///     Gets or sets the byte at the specified index, with the least significant byte having index 0
        /// </summary>
        /// <param name="index"></param>
        public byte this[int index]
        {
            get => Bytes[Bytes.Length - (index + 1)];
            set => Bytes[Bytes.Length - (index + 1)] = value;
        }

        /// <summary>
        ///     Returns a sequence of "count" bytes following the startIndex, with the least significant byte having index 0
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public byte[] this[int startIndex, int count] =>
            Enumerable.Range(startIndex, count).Select(i => this[i]).Reverse().ToArray();

        public int CompareTo(uintn b)
        {
            if (ReferenceEquals(b, null))
                return 1;
            if (GetType() != b.GetType())
                throw new InvalidOperationException($"Cannot compare {GetType()} with {b.GetType()}");
            if (_numBytes != b._numBytes)
                throw new InvalidOperationException(
                    $"Cannot compare values with differing byte sizes ({_numBytes},{b._numBytes})");

            for (var i = 0; i < _numBytes; i++)
            {
                if (Bytes[i] == b.Bytes[i]) continue;
                if (Bytes[i] < b.Bytes[i]) return -1;
                if (Bytes[i] > b.Bytes[i]) return 1;
            }

            return 0;
        }

        /// <summary>
        ///     Converts an instance of this class into its hexadecimal representation.
        /// </summary>
        /// <returns>The hex.</returns>
        public string ToHex()
        {
            return Hex.FromByteArray(Bytes);
        }

        public override int GetHashCode()
        {
            return Bytes.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return CompareTo(obj as uintn) == 0;
        }

        public static bool operator <(uintn a, uintn b)
        {
            return a?.CompareTo(b) < 0;
        }

        public static bool operator >(uintn a, uintn b)
        {
            return a?.CompareTo(b) > 0;
        }

        public static bool operator ==(uintn a, uintn b)
        {
            return a?.CompareTo(b) == 0;
        }

        public static bool operator !=(uintn a, uintn b)
        {
            return a?.CompareTo(b) != 0;
        }

        public static bool operator <=(uintn a, uintn b)
        {
            return a?.CompareTo(b) <= 0;
        }

        public static bool operator >=(uintn a, uintn b)
        {
            return a?.CompareTo(b) >= 0;
        }
    }

    /// <summary>
    ///     Private keys and hashes should be stored in this type.
    /// </summary>
    public class uint256 : uintn
    {
        public uint256(string value) : base(32, value)
        {
        }

        public uint256(byte[] value) : base(32, value)
        {
        }

        public uint256(uint256 value) : this(value.Bytes)
        {
        }
    }

    public class uint160 : uintn
    {
        public uint160(string value) : base(20, value)
        {
        }

        public uint160(byte[] value) : base(20, value)
        {
        }

        public uint160(uint160 value) : this(value.Bytes)
        {
        }
    }
}