using System.Net;

namespace GameNetwork.Core.Messages
{
    public class RawByteMessage
    {
        public RawByteMessage(IPEndPoint sender, byte[] data, int length)
        {
            Sender = sender;
            Data = data;
            Length = length;
        }

        public IPEndPoint Sender { get; }
        public int Length { get; }
        public byte[] Data { get; }
    }
}