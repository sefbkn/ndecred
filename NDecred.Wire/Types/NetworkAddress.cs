﻿using System;
using System.IO;
using System.Linq;
using NDecred.Common;

namespace NDecred.Wire
{
    public class NetworkAddress : NetworkEncodable
    {
        private readonly bool _useTimestamp;

        public NetworkAddress(bool useTimestamp)
        {
            _useTimestamp = useTimestamp;
            Ip = new byte[16];
        }
        
        public DateTime Timestamp { get; set; }
        public ServiceFlag Services { get; set; }
        public byte[] Ip { get; set; }
        public ushort Port { get; set; }

        public void AddService(ServiceFlag service)
        {
            Services |= service;
        }

        public override void Decode(BinaryReader reader)
        {
            if (_useTimestamp)
            {
                Timestamp = DateTimeExtensions.FromUnixTime(reader.ReadUInt32());                
            }
            
            Services = (ServiceFlag) reader.ReadUInt64();
            Ip = reader.ReadBytes(16);
            Port = DecodePort(reader);
        }

        public override void Encode(BinaryWriter writer)
        {
            if (_useTimestamp)
            {
                writer.Write((uint) Timestamp.ToUnixTime());                
            }
            
            writer.Write((ulong) Services);

            EncodeIpAddress(writer);
            EncodePort(writer);
        }

        private void EncodeIpAddress(BinaryWriter writer)
        {
            var ipOut = new byte[16];
            var ipRawBytes = Ip;
            for (var i = 0; i < ipOut.Length && i < ipRawBytes.Length; i++)
                ipOut[i] = ipRawBytes[i];
            writer.Write(ipOut);
        }

        private ushort DecodePort(BinaryReader reader)
        {
            // Port is encoded in big endian format...
            var portBytes = reader.ReadBytes(2).Reverse().ToArray();
            return BitConverter.ToUInt16(portBytes, 0);
        }

        private void EncodePort(BinaryWriter writer)
        {
            // Port is encoded in big endian format...
            var portBytes = BitConverter.GetBytes(Port).Reverse().ToArray();
            writer.Write(portBytes);
        }
    }
}