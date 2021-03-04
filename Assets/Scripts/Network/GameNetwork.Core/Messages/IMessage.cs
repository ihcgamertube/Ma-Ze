using System;
using System.Runtime.Serialization;
using GameNetwork.Core.Messages.Custom;

namespace GameNetwork.Core.Messages
{
    public interface IMessage
    {
        Guid Id { get; }
        MessageType MessageType { get; }
    }
}