using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GameNetwork.Core.Messages.Custom.Stubs
{
    public class PlayerCoordinatesMessage : IMessage
    {
        public PlayerCoordinatesMessage(Guid id, MessageType messageType, Vector3 position, Quaternion rotation)
        {
            if (messageType != MessageType.PlayerCoordinates) throw new ArgumentException(nameof(messageType));
            Id = id;
            MessageType = messageType;
            Position = position;
            Rotation = rotation;
        }

        public PlayerCoordinatesMessage(Guid id, Vector3 position, Quaternion rotation) : this(id, MessageType.PlayerCoordinates, position, rotation)
        {

        }

        public Guid Id { get; }
        public MessageType MessageType { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
    }
}
