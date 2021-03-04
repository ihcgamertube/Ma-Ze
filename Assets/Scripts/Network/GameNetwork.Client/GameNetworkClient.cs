using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.InteropServices;
using GameNetwork.Core;
using GameNetwork.Core.Messages;
using GameNetwork.Core.Messages.Custom;
using GameNetwork.Core.Messages.Custom.Stubs;

namespace GameNetwork.Client
{
    public class GameNetworkClient
    {
        public static GameNetworkClient Connect(string ip, int port)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            return Connect(endPoint);
        }

        public static GameNetworkClient Connect(IPEndPoint endPoint)
        {
            var tcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcp.Connect(endPoint);

            var udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udp.Bind(tcp.LocalEndPoint);
            udp.Connect(endPoint);

            var buffer = new byte[1024];
            var idMessage = tcp.Receive(buffer);
            var serverAcceptedMessage = ByteConverters.FromBytes<ServerAcceptanceMessage>(buffer);
            if (serverAcceptedMessage.DenyReason != 0) throw new Exception("Server denied");

            return new GameNetworkClient(serverAcceptedMessage.Id, serverAcceptedMessage.ClientId, tcp, udp);
        }

        private readonly Player _player;
        private SocketReceiverFromAgent _udpReceiver;
        private SocketReceiverAgent _tcpReceiver;
        private Action<Message> _messageReceivedCallback;

        private GameNetworkClient(Guid serverId, Guid id, Socket tcp, Socket udp)
        {
            ServerId = serverId;
            _player = new Player(id, (IPEndPoint)tcp.LocalEndPoint, (IPEndPoint)tcp.RemoteEndPoint, tcp, udp);
            _udpReceiver = new SocketReceiverFromAgent(_player.Udp, Receiver);
            _tcpReceiver = new SocketReceiverAgent(_player.Tcp, Receiver);

            //_udpReceiver.Start();
            //_tcpReceiver.Start();
        }

        private void Receiver(RawByteMessage raw)
        {
            var message = new Message(
                Utils.GetGuid(raw.Data, 0),
                (MessageType)BitConverter.ToInt16(raw.Data, 16),
                raw.Data);
            _messageReceivedCallback?.Invoke(message);
        }

        public Guid Id => _player.Id;
        public Guid ServerId { get; }

        public void SetMessageReceivedCallback(Action<Message> callback)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            _messageReceivedCallback = callback;
        }

        public void SendCoordinates(Vector3 position, Quaternion rotation)
        {
            _player.SendFast(new PlayerCoordinatesMessage(Id, position, rotation));
        }

        public void Disconnect()
        {
            _player.Disconnect(0xff);
        }
    }
}