﻿namespace HomeCenter.Core.Interface.Messaging
{
    public interface IUdpBroadcastMessage
    {
        string MessageAddress();

        byte[] Serialize();
    }
}