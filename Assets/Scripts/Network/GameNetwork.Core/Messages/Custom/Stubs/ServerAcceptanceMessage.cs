using System;
using System.Runtime.InteropServices;

namespace GameNetwork.Core.Messages.Custom
{
    public class ServerAcceptanceMessage : IMessage
    {
        public ServerAcceptanceMessage(Guid id, MessageType messageType, int denyReason, Guid clientId)
        {
            if (messageType != MessageType.ServerAccepted) throw new ArgumentException(nameof(MessageType));
            Id = id;
            MessageType = MessageType.ServerAccepted;
            DenyReason = denyReason;
            ClientId = clientId;
        }

        public ServerAcceptanceMessage(Guid id, int denyReason, Guid clientId)
        {
            Id = id;
            MessageType = MessageType.ServerAccepted;
            DenyReason = denyReason;
            ClientId = clientId;
        }

        public Guid Id { get; }
        public MessageType MessageType { get; }
        public int DenyReason { get; }
        public Guid ClientId { get; }
    }
}