using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GameNetwork.Core.Messages.Custom.Stubs
{
    public class ServerCoordinatesMessage : IMessage
    {
        public ServerCoordinatesMessage(Guid id, MessageType messageType, PlayerCoordination[] coordinates)
        {
            Id = id;
            MessageType = messageType;
            Coordinates = coordinates;
            Count = coordinates.Length;
        }

        public ServerCoordinatesMessage(Guid id, PlayerCoordination[] coordinates)
        {
            Id = id;
            Coordinates = coordinates;
            MessageType = MessageType.ServerCoordinates;
            Count = coordinates.Length;
        }

        public Guid Id { get; }
        public MessageType MessageType { get; }
        public int Count { get; }
        public PlayerCoordination[] Coordinates { get; }
    }

    public struct PlayerCoordination
    {
        public PlayerCoordination(Guid player, Vector3 position, Quaternion rotation)
        {
            Player = player;
            Position = position;
            Rotation = rotation;
        }

        public Guid Player { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
    }
}
