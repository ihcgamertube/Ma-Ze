using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;
using GameNetwork.Core.Messages;

namespace GameNetwork.Core
{
    public class SocketReceiverFromAgent : SocketReceiverAgent
    {
        public SocketReceiverFromAgent(Socket socket, Action<RawByteMessage> receiver, Action<Exception> exceptionHandler = null)
            : base(socket, receiver, exceptionHandler)
        {
        }

        public SocketReceiverFromAgent(Socket socket, ITargetBlock<RawByteMessage> receiver, Action<Exception> exceptionHandler = null)
            : base(socket, receiver, exceptionHandler)
        {
        }

        protected override RawByteMessage GetMessage()
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            var bytesReceived = _socket.ReceiveFrom(_buffer, ref endPoint);
            var result = new byte[bytesReceived];
            Array.Copy(_buffer, result, bytesReceived);
            return new RawByteMessage((IPEndPoint)endPoint, result, result.Length);
        }
    }
}