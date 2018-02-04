using System;
using System.IO;

namespace NDecred.Network
{
    public abstract class NetworkMessage
    {
        protected NetworkMessage()
        {
        }

        /// <summary>
        /// Populates the current instances with the encoded data supplied as a byte[]
        /// </summary>
        /// <param name="bytes"></param>
        public void Decode(byte[] bytes)
        {
            using(var ms = new MemoryStream(bytes))
            using(var br = new BinaryReader(ms))
            {
                Decode(br);
            }
        }

        /// <summary>
        /// Serializes the current instance and returns the value as a byte[]
        /// </summary>
        /// <returns></returns>
        public byte[] Encode()
        {
            using(var ms = new MemoryStream())
            using(var bw = new BinaryWriter(ms))
            {
                Encode(bw);
                bw.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Populates the current instance with values read from the reader
        /// </summary>
        /// <param name="reader"></param>
        public abstract void Decode(BinaryReader reader);   
        
        /// <summary>
        /// Serializes the current instance and writes the output to the writer.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Encode(BinaryWriter writer);
    }
}