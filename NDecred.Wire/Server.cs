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

        private void PeerMessageReceived(Peer sender, PeerMessageReceivedArgs e)
        {
            switch (e.Message)
            {
                case MsgAddr addr:
                    OnMsgAddressReceived(sender, new PeerMessageReceivedArgs<MsgAddr>(e.Header, addr));
                    break;
                case MsgGetAddr getAddr:
                    sender.SendMessage(new MsgAddr { });
                    break;
                case MsgPing ping:
                    sender.SendMessage(new MsgPong { Nonce = ping.Nonce });
                    break;
                case MsgPong pong:
                    break;
                case MsgVerAck verAck:
                    Peers.Add(sender);
                    break;
                case MsgVersion version:
                    break;
                case MsgReject reject:
                    break;
                case MsgTx tx:
                    break;
                case MsgBlock block:
                    break;
            }
        }

        public virtual void OnMsgAddressReceived(Peer peer, PeerMessageReceivedArgs<MsgAddr> e)
        {
            foreach (var a in e.Message.Addresses)
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        var endpoint = new IPEndPoint(new IPAddress(a.Ip), a.Port);
                        ConnectToPeerAsync(endpoint).Wait();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                });
                        
                thread.Start();
            }
        }

        public void Dispose()
        {
            foreach (var peer in Peers)
            {
                peer.MessageReceived -= PeerMessageReceived;
                peer.Dispose();
            }                
        }
    }
}