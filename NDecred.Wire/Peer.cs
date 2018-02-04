using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Subjects;

namespace NDecred.Wire
{
    /// <summary>
    /// WIP.  No error handling / single threaded + doesn't react to messages.
    /// </summary>
    public class Peer
    {        
        public void Connect()
        {            
            using(var client = new TcpClient("localhost", 18555))
            using(var stream = client.GetStream())
            using(var writer = new BinaryWriter(stream))
            {
                SendVersion(writer);
                HandleIncomingMessages(client);
            }
        }
        
        private void SendVersion(BinaryWriter writer)
        {
            var version = new MsgVersion
            {
                ProtocolVersion = 1,
            };
                
            var versionBytes = version.Encode();
            var header = new MessageHeader(CurrencyNet.SimNet, MsgCommand.Version, versionBytes);
            var headerBytes = header.Encode();
            writer.Write(headerBytes.Concat(versionBytes).ToArray());
            writer.Flush();
        }

        private IEnumerable<(MessageHeader header, NetworkEncodable body)> IncomingMessages(TcpClient client)
        {
            var stream = client.GetStream();
            using (var reader = new BinaryReader(stream))
            {
                while (client.Connected)
                {                    
                    var msgHeader = new MessageHeader();
                    msgHeader.Decode(reader);

                    var message = msgHeader.Command.CreateMessage();
                    message.Decode(reader);

                    yield return (msgHeader, message);
                }                
            }
        }

        private void HandleIncomingMessages(TcpClient client)
        {
            foreach (var message in IncomingMessages(client))
            {
                switch (message.body)
                {
                    case MsgPing ping:
                        break;
                    case MsgPong pong:
                        break;
                    case MsgVerAck verAck:
                        break;
                    case MsgVersion version:
                        break;
                    case MsgReject reject:
                        break;
                }                
            }
        }
    }
}

