using System;
using System.Runtime.InteropServices;

namespace GameNetwork.Core.Messages.Custom
{
    public class Message : IMessage
    {
        public static readonly int MessageHeaderSize = Marshal.SizeOf<Guid>() + Marshal.SizeOf<short>();

        public Message(Guid id, MessageType messageType, byte[] data)
        {
            Id = id;
            MessageType = messageType;
            Data = data;
        }

        public Guid Id { get; }
        public MessageType MessageType { get; }
        public int MessageLength => MessageHeaderSize;
        public byte[] Data { get; }
    }
}