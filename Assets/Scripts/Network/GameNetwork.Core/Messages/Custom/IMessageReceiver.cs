using System;
using System.Collections.Generic;
using System.Text;
using GameNetwork.Core.Messages.Custom;
using GameNetwork.Core.Messages.Custom.Stubs;

namespace GameNetwork.Core.Messages
{
    public interface IMessageReceiver
    {
        void Receive(IMessage message);
    }
}
