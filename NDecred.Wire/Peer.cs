using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NDecred.Core;

namespace NDecred.Wire
{
    /// <summary>
    /// WIP.  No error handling / single threaded + doesn't react to messages.
    /// </summary>
    public class Peer : IDisposable
    {
        private CurrencyNet CurrencyNet;
        private TcpClient _client;
        private Task _backgroundReadTask;
        
        public IPEndPoint IpEndPoint { get; }
        
        public Peer()
        {
            CurrencyNet = CurrencyNet.SimNet;
            IpEndPoint = new IPEndPoint(new IPAddress(new byte[]{127,0,0,1}), 18555);
        }

        public async Task ConnectAsync()
        {
            if (_client != null && _client.Connected)
                return;

            _client = new TcpClient();
            await _client.ConnectAsync(IpEndPoint.Address, IpEndPoint.Port);
            
            // Fire off background task to process incoming requests
            _backgroundReadTask = new Task(HandleIncomingMessages, TaskCreationOptions.LongRunning);
            _backgroundReadTask.ConfigureAwait(false);
            _backgroundReadTask.Start();
            
            SendVersion();
        }
        
        private void SendVersion()
        {
            using (var writer = new BinaryWriter(_client.GetStream(), Encoding.Default, true))
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
        }

        /// <summary>
        /// Blocks until data is received from the peer represented by this instance.
        /// Messages are parsed and yielded as soon as they are available on the wire.
        /// </summary>
        /// <returns>a blocking enumerable that can be iterated over</returns>
        private IEnumerable<(MessageHeader header, NetworkEncodable body)> IncomingMessages()
        {
            var stream = _client.GetStream();
            var reader = new BinaryReader(stream);
            
            while (_client.Connected)
            {
                var msgHeader = new MessageHeader();
                msgHeader.Decode(reader);

                var message = msgHeader.Command.CreateMessage();
                message.Decode(reader);

                yield return (msgHeader, message);
            }
        }

        /// <summary>
        /// Waits for incoming messages to be available, and handles each one based on its type.
        /// </summary>
        private void HandleIncomingMessages()
        {
            foreach (var message in IncomingMessages())
            {
                switch (message.body)
                {
                    case MsgPing ping:
                        OnPing(ping);
                        break;
                    case MsgPong pong:
                        break;
                    case MsgVerAck verAck:
                        break;
                    case MsgVersion version:
                        break;
                    case MsgReject reject:
                        break;
                    case MsgTx tx:
                        break;
                    case MsgBlock block:
                        break;
                    case MsgBlockHeader blockHeader:
                        break;
                }                
            }
        }

        private void OnPing(MsgPing ping)
        {
            var stream = _client.GetStream();
            var writer = new BinaryWriter(stream);
            
            var msgPong = new MsgPong { Nonce = ping.Nonce };
            var msgHeader = new MessageHeader(CurrencyNet, MsgCommand.Pong, msgPong.Encode());
            msgHeader.Encode(writer);
            msgPong.Encode(writer);
            writer.Flush();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}

