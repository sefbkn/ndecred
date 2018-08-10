using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NDecred.Wire
{
    public class Server : IDisposable
    {
        public Server()
        {
            Peers = new List<Peer>();
        }
        
        public List<Peer> Peers { get; }

        public async Task<Peer> ConnectToPeerAsync(IPEndPoint endpoint)
        {
            var networkClient = new NetworkClient(endpoint.Address.GetAddressBytes(), endpoint.Port);
            var peer = new Peer(networkClient, CurrencyNet.TestNet2);
            
            // Subscribe to incoming messages
            peer.MessageReceived += PeerMessageReceived;
            
            // Establish connection
            await peer.ConnectAsync();
            
            return peer;
        }

        /// <summary>
        /// Called whenever a fully parsed message is received from a peer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeerMessageReceived(Peer sender, PeerMessageReceivedArgs e)
        {
            switch (e.Message)
            {
                case MsgAddr msgAddr:
                    break;
                case MsgBlock msgBlock:
                    break;
                case MsgGetAddr msgGetAddr:
                    sender.SendMessage(new MsgAddr { });
                    break;
                case MsgGetBlocks msgGetBlocks:
                    break;
                case MsgGetHeaders msgGetHeaders:
                    break;                
                case MsgGetMiningState msgGetMiningState:
                    break;
                case MsgPing msgPing:
                    sender.SendMessage(new MsgPong { Nonce = msgPing.Nonce });
                    break;
                case MsgPong msgPong:
                    break;
                case MsgReject msgReject:
                    break;
                case MsgTx msgTx:
                    break;
                case MsgVerAck msgVerAck:
                    Peers.Add(sender);
                    break;
                case MsgVersion msgVersion:
                    break;
            }
        }

        public void Dispose()
        {
            // Unsubscribe from peer events
            foreach (var peer in Peers)
            {
                peer.MessageReceived -= PeerMessageReceived;
            }

            foreach (var peer in Peers)
            {
                peer.Dispose();
            }
        }
    }
}