using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GameNetwork.Core
{
    public static class Utils
    {
        private static readonly int GuidSize = Marshal.SizeOf<Guid>();
        public static Guid GetGuid(byte[] value, int start)
        {
            if (value.Length + start < GuidSize)
                throw new InvalidOperationException();
            return new Guid(
                BitConverter.ToInt32(value, start + 0),
                BitConverter.ToInt16(value, start + 4),
                BitConverter.ToInt16(value, start + 6),
                value[start + 8],
                value[start + 9],
                value[start + 10],
                value[start + 11],
                value[start + 12],
                value[start + 13],
                value[start + 14],
                value[start + 15]);
        }

        public static T FromBytes<T>(byte[] arr,int startIndex = 0) where T : struct
        {
            var structure = new T();

            var size = Marshal.SizeOf(structure);
            var ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, startIndex, ptr, size);

            structure = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);

            return structure;
        }

        public static byte[] GetBytes<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf(structure);
            byte[] arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static IEnumerable<T> PadRight<T>(this IEnumerable<T> source, int length)
        {
            int i = 0;
            // use "Take" in case "length" is smaller than the source's length.
            foreach (var item in source.Take(length))
            {
                yield return item;
                i++;
            }
            for (; i < length; i++)
                yield return default(T);
        }

        public static int SetRange(this byte[] array, IEnumerable<byte> range, int start = 0)
        {
            int i = 0;
            foreach (var b in range)
            {
                array[start + i] = b;
                i++;
            }

            return i;
        }

        public static int Set(this byte[] array, int n, int start)
        {
            array[start + 0] = (byte)((n >> 24) & 0xFF);
            array[start + 1] = (byte)((n >> 16) & 0xFF);
            array[start + 2] = (byte)((n >> 8) & 0xFF);
            array[start + 3] = (byte)(n & 0xFF);
            return 4;
        }

        public static int Set(this byte[] array, short n, int start)
        {
            array[start + 0] = (byte)((n >> 24) & 0xFF);
            array[start + 1] = (byte)((n >> 16) & 0xFF);
            return 2;
        }
    }
}