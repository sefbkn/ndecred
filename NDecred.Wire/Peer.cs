using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NDecred.Wire
{
    /// <summary>
    /// WIP.  No error handling / single threaded + doesn't react to messages.
    /// </summary>
    public class Peer : IDisposable
    {
        private readonly CurrencyNet _currencyNet;
        private readonly INetworkClient _client;
        private Task _backgroundReadTask;

        public Peer(INetworkClient networkClient, CurrencyNet currencyNet)
        {
            _client = networkClient ?? throw new ArgumentNullException(nameof(networkClient));
            _currencyNet = currencyNet;
        }

        public delegate void EventHandler<in TSender, in TArgs>(TSender sender, TArgs e) where TArgs : EventArgs;
        public event EventHandler<Peer, PeerMessageReceivedArgs> MessageReceived;
        
        /// <summary>
        /// Asynchronously establishes a connection with the peer.  The version is sent
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            if (_client.IsConnected)
                return;

            await _client.ConnectAsync();

            // Before any other messages can be processed, need to send version
            SendMessage(new MsgVersion { ProtocolVersion = 1 });
            
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

            using (var reader = _client.GetStreamReader())
            {
                while (true)
                {
                    // If there is no data to read, this will block until the next message becomes available.
                    var header = new MessageHeader();
                    header.Decode(reader);

                    var message = header.Command.CreateMessage();
                    message.ProtocolVersion = 1;
                    message.Decode(reader);
                
                    OnMessageReceived(new PeerMessageReceivedArgs(header, message));
                }
            }
        }

        /// <summary>
        /// Sends a message to the peer represented by this instance.
        /// 
        /// A message header is created for the message, internally, and also sent with the provided message
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(Message message)
        {
            using (var writer = _client.GetStreamWriter())
            {
                var messageBytes = message.Encode();
                var messageHeader = new MessageHeader(_currencyNet, message.Command, messageBytes);
                var messageHeaderBytes = messageHeader.Encode();
            
                writer.Write(messageHeaderBytes.Concat(messageBytes).ToArray());
                writer.Flush();
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

        public override string ToString()
        {
            return $"Peer {_client}; Connected: {_client.IsConnected}";
        }
    }
}

