using System;
using System.Net;
using System.Net.Sockets;
using GameNetwork.Core.Messages;

namespace GameNetwork.Core
{
    public class Player
    {
        public Player(Guid id, IPEndPoint localAddress, IPEndPoint remoteAddress, Socket tcp, Socket udp)
        {
            Id = id;
            Tcp = tcp;
            Udp = udp;
            LocalAddress = localAddress;
            RemoteAddress = remoteAddress;
        }

        public void SendFast<T>(T message) where T : IMessage
        {
            Udp.SendTo(ByteConverters.ToBytes(message), RemoteAddress);
        }

        public void SendReliable<T>(T message) where T : IMessage
        {
            Tcp.Send(ByteConverters.ToBytes(message));
        }

        public void Disconnect(int reason)
        {
            //TODO handle reason
            Udp.Close();

            Tcp.Disconnect(false);
            Tcp.Close();
        }

        public Guid Id { get; }
        public IPEndPoint LocalAddress { get; }
        public IPEndPoint RemoteAddress { get; }
        public Socket Tcp { get; }
        public Socket Udp { get; }
    }
}