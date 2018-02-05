﻿using System;
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
            endpoint.Address = endpoint.Address.MapToIPv4();
            var tcpClient = new TcpClient(endpoint.Address.AddressFamily);
            var peer = new Peer(tcpClient, CurrencyNet.TestNet2);
            peer.MessageReceived += PeerMessageReceived;
            await peer.ConnectAsync(endpoint);            
            return peer;
        }

        private async void PeerMessageReceived(Peer sender, PeerMessageReceivedArgs e)
        {
            switch (e.Message)
            {
                case MsgAddr addr:
                    foreach (var a in addr.Addresses)
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
                    break;
                case MsgGetAddr getAddr:
                    await sender.SendMessageAsync(new MsgAddr { });
                    break;
                case MsgPing ping:
                    await sender.SendMessageAsync(new MsgPong { Nonce = ping.Nonce });
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