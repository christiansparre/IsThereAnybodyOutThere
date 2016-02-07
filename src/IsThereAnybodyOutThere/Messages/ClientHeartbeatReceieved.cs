using System;
using System.Collections.Generic;

namespace IsThereAnybodyOutThere.Messages
{
    public class ClientHeartbeatReceieved
    {
        public string ClientId { get; }
        public DateTime ReceivedAtUtc { get; }

        public Dictionary<string, object> Heartbeat { get; }

        public ClientHeartbeatReceieved(string clientId, DateTime receivedAtUtc, Dictionary<string, object> heartbeat)
        {
            ClientId = clientId;
            ReceivedAtUtc = receivedAtUtc;
            Heartbeat = heartbeat;
        }
    }

    public class ClientInfoReceived
    {
        public string ClientId { get; }
        public DateTime ReceivedAtUtc { get; }

        public Dictionary<string, object> Info { get; }

        public ClientInfoReceived(string clientId, DateTime receivedAtUtc, Dictionary<string, object> info)
        {
            ClientId = clientId;
            ReceivedAtUtc = receivedAtUtc;
            Info = info;
        }
    }
 }