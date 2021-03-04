using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using GameNetwork.Core.Messages;

namespace GameNetwork.Core
{
    public class SocketReceiverAgent
    {
        protected readonly Socket _socket;
        protected readonly byte[] _buffer = new byte[1024];
        private readonly Action<RawByteMessage> _receiver;
        private readonly Action<Exception> _exceptionHandler;

        public SocketReceiverAgent(
        Socket socket,
        Action<RawByteMessage> receiver,
        Action<Exception> exceptionHandler = null)
        {
            _socket = socket;
            _receiver = receiver;
            _exceptionHandler = exceptionHandler;

            Task = Task.Run(Run);
        }

        public SocketReceiverAgent(
            Socket socket,
            ITargetBlock<RawByteMessage> receiver,
            Action<Exception> exceptionHandler = null)
            : this(
                socket,
                (m) => receiver.Post(m),
                exceptionHandler)
        {

        }

        public Task Task { get; }

        private void Run()
        {
            while (true)
            {
                try
                {
                    var message = GetMessage();
                    _receiver(message);
                }
                catch (Exception ex)
                {
                    if (ex is SocketException socketEx &&
                        socketEx.ErrorCode == 10004 &&
                        socketEx.SocketErrorCode == SocketError.Interrupted)
                    {
                        break;
                    }

                    if (_exceptionHandler != null)
                    {
                        _exceptionHandler(ex);
                        break;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            Trace.WriteLine("UDP receiver thread done.");
        }

        protected virtual RawByteMessage GetMessage()
        {
            var bytesReceived = _socket.Receive(_buffer);
            var result = new byte[bytesReceived];
            Array.Copy(_buffer, result, bytesReceived);
            return new RawByteMessage((IPEndPoint)_socket.RemoteEndPoint, result, result.Length);

        }
    }
}
