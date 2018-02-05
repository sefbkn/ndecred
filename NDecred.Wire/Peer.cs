using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
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
        private readonly CurrencyNet _currencyNet;
        private readonly TcpClient _client;
        private Task _backgroundReadTask;
                                
        public Peer(TcpClient tcpClient, CurrencyNet currencyNet)
        {
            _client = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            _currencyNet = currencyNet;
        }

        public delegate void EventHandler<in TSender, in TArgs>(TSender sender, TArgs e) where TArgs : EventArgs;
        public event EventHandler<Peer, PeerMessageReceivedArgs> MessageReceived;
        
        /// <summary>
        /// Asynchronously establishes a connection with the peer.  The version is sent
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync(IPEndPoint ipEndPoint)
        {
            if (_client.Connected)
                return;

            await _client.ConnectAsync(ipEndPoint.Address, ipEndPoint.Port);

            // Before any other messages can be processed, need to send version
            await SendMessageAsync(new MsgVersion { ProtocolVersion = 1});
            
            // Fire off background task to process incoming requests
            _backgroundReadTask = new Task(ReadIncomingMessages, TaskCreationOptions.LongRunning);
            _backgroundReadTask.Start();
        }
        
        /// <summary>
        /// Blocks until data is received from the peer represented by this instance.
        /// Messages are parsed as soon as they are available on the wire, and broadcast
        /// via the MessageReceived event.
        /// </summary>
        private void ReadIncomingMessages()
        {
            var stream = _client.GetStream();
            var reader = new BinaryReader(stream);
            
            while (_client.Connected)
            {
                var header = new MessageHeader();
                header.Decode(reader);

                var message = header.Command.CreateMessage();
                message.ProtocolVersion = 1;
                message.Decode(reader);
                
                OnMessageReceived(new PeerMessageReceivedArgs(header, message));
            }
        }

        /// <summary>
        /// Sends a message to the peer represented by this instance.
        /// 
        /// A message header is created for the message, internally, and also sent with the provided message
        /// </summary>
        /// <param name="message"></param>
        public async Task SendMessageAsync(Message message)
        {
            using(var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                var messageBytes = message.Encode();
                var messageHeader = new MessageHeader(_currencyNet, message.Command, messageBytes);
                var messageHeaderBytes = messageHeader.Encode();
            
                writer.Write(messageHeaderBytes.Concat(messageBytes).ToArray());
                writer.Flush();

                var bytes = ms.ToArray();
                await _client.GetStream().WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        protected virtual void OnMessageReceived(PeerMessageReceivedArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}

