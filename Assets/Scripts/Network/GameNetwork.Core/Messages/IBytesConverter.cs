using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using GameNetwork.Core.Messages.Custom;
using GameNetwork.Core.Messages.Custom.Stubs;

namespace GameNetwork.Core.Messages
{
    public interface IBytesConverter
    {
        byte[] ToBytes(object value);
        object FromBytes(byte[] bytes);
        int EstimateSize(object value);
        int MaxPossibleSize { get; }
    }

    public abstract class ByteConverter<T> : IBytesConverter
    {
        public byte[] ToBytes(object value)
        {
            return ToBytes((T)value);
        }

        public abstract byte[] ToBytes(T value);

        public object FromBytes(byte[] bytes)
        {
            return ValueFromBytes(bytes);
        }

        public abstract T ValueFromBytes(byte[] bytes);

        public int EstimateSize(object value)
        {
            return EstimateSize((T)value);
        }

        public abstract int EstimateSize(T value);

        public abstract int MaxPossibleSize { get; }
    }

    public class ServerAcceptanceMessageConverter : ByteConverter<ServerAcceptanceMessage>
    {
        private static readonly int Size = Marshal.SizeOf<Guid>() +
                                           Marshal.SizeOf<short>() +
                                           Marshal.SizeOf<int>() +
                                           Marshal.SizeOf<Guid>();

        public override byte[] ToBytes(ServerAcceptanceMessage value)
        {
            var size = EstimateSize(value);
            var bytes = new byte[size];
            var index = 0;
            index += bytes.SetRange(value.Id.ToByteArray());
            index += bytes.SetRange(BitConverter.GetBytes((short)value.MessageType), index);
            index += bytes.SetRange(BitConverter.GetBytes(value.DenyReason), index);
            bytes.SetRange(value.ClientId.ToByteArray(), index);
            return bytes;
        }

        public override ServerAcceptanceMessage ValueFromBytes(byte[] bytes)
        {
            return new ServerAcceptanceMessage(
                Utils.FromBytes<Guid>(bytes),
                (MessageType)BitConverter.ToInt16(bytes, 16),
                BitConverter.ToInt32(bytes, 18),
                Utils.FromBytes<Guid>(bytes, 22)
            );
        }

        public override int EstimateSize(ServerAcceptanceMessage value) => Size;

        public override int MaxPossibleSize => Size;
    }

    public class PlayerCoordinatesMessageConverter : ByteConverter<PlayerCoordinatesMessage>
    {
        private static readonly int Size = Marshal.SizeOf<Guid>() + Marshal.SizeOf<short>() +
                                           Marshal.SizeOf<Vector3>() + Marshal.SizeOf<Quaternion>();

        public override byte[] ToBytes(PlayerCoordinatesMessage value)
        {
            var size = EstimateSize(value);
            var bytes = new byte[size];
            var index = 0;
            index += bytes.SetRange(value.Id.ToByteArray());
            index += bytes.SetRange(BitConverter.GetBytes((short)value.MessageType), index);
            index += bytes.SetRange(Utils.GetBytes(value.Position), index);
            bytes.SetRange(Utils.GetBytes(value.Rotation), index);
            return bytes;
        }

        public override PlayerCoordinatesMessage ValueFromBytes(byte[] bytes)
        {
            return new PlayerCoordinatesMessage(
                Utils.FromBytes<Guid>(bytes),
                (MessageType)BitConverter.ToInt16(bytes, 16),
                Utils.FromBytes<Vector3>(bytes, 18),
                Utils.FromBytes<Quaternion>(bytes, 18 + Marshal.SizeOf<Vector3>()));
        }

        public override int EstimateSize(PlayerCoordinatesMessage value) => Size;

        public override int MaxPossibleSize => Size;
    }

    public class ServerCoordinatesMessageConverter : ByteConverter<ServerCoordinatesMessage>
    {
        private static readonly int QuaternionSize = Marshal.SizeOf<Quaternion>();
        private static readonly int Vector3Size = Marshal.SizeOf<Vector3>();
        private static readonly int GuidSize = Marshal.SizeOf<Guid>();
        private static readonly int PlayerCoordinationSize = Marshal.SizeOf<PlayerCoordination>();
        private static readonly int BaseSize = Marshal.SizeOf<Guid>() + Marshal.SizeOf<short>() + Marshal.SizeOf<int>();


        public override byte[] ToBytes(ServerCoordinatesMessage value)
        {
            var size = EstimateSize(value);
            var bytes = new byte[size];
            var index = 0;
            index += bytes.SetRange(value.Id.ToByteArray());
            index += bytes.SetRange(BitConverter.GetBytes((short)value.MessageType), index);
            index += bytes.SetRange(BitConverter.GetBytes(value.Count), index);


            for (int i = 0; i < value.Count; i++)
            {
                index += bytes.SetRange(Utils.GetBytes(value.Coordinates[i]), index);
            }

            return bytes;
        }

        public override ServerCoordinatesMessage ValueFromBytes(byte[] bytes)
        {
            int index = 0;

            var id = Utils.FromBytes<Guid>(bytes);
            index += GuidSize;

            var type = (MessageType)BitConverter.ToInt16(bytes, index);
            index += 2;

            var count = BitConverter.ToInt32(bytes, index);
            index += 4;

            var coordinates = new PlayerCoordination[count];

            for (int i = 0; i < count; i++)
            {
                var playerCoordination = Utils.FromBytes<PlayerCoordination>(bytes, index);
                coordinates[i] = playerCoordination;
                index += PlayerCoordinationSize;
            }

            return new ServerCoordinatesMessage(id, type, coordinates);
        }

        public override int EstimateSize(ServerCoordinatesMessage value)
        {
            return BaseSize + (Vector3Size + QuaternionSize + GuidSize) * value.Count;
        }

        public override int MaxPossibleSize => BaseSize + (Vector3Size + QuaternionSize + GuidSize) * 4;
    }

    public static class ByteConverters
    {
        private static IReadOnlyDictionary<Type, IBytesConverter> _converters =
            new ReadOnlyDictionary<Type, IBytesConverter>(new Dictionary<Type, IBytesConverter>()
            {
                [typeof(ServerAcceptanceMessage)] = new ServerAcceptanceMessageConverter(),
                [typeof(PlayerCoordinatesMessage)] = new PlayerCoordinatesMessageConverter(),
                [typeof(ServerCoordinatesMessage)] = new ServerCoordinatesMessageConverter(),
            });

        public static T FromBytes<T>(byte[] bytes)
        {
            var converter = _converters[typeof(T)];
            if (converter is ByteConverter<T> tConverter)
                return tConverter.ValueFromBytes(bytes);
            return (T)_converters[typeof(T)].FromBytes(bytes);
        }

        public static byte[] ToBytes<T>(T value)
        {
            var converter = _converters[typeof(T)];
            if (converter is ByteConverter<T> tConverter)
                return tConverter.ToBytes(value);
            return _converters[typeof(T)].ToBytes(value);
        }
    }
}
