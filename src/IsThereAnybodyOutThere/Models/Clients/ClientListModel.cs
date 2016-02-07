using System;
using System.Collections.Generic;

namespace IsThereAnybodyOutThere.Models.Clients
{
    public class ClientModel
    {
        public string Id { get; set; }
        public bool Connected { get; set; }

        public DateTime LastHeartbeatReceivedAtUtc { get; set; }
        public string MachineName { get; set; }
        public string Username { get; set; }
        public string ApplicationName { get; set; }
        public IEnumerable<Heartbeat> Heartbeats { get; set; }

        public class Heartbeat
        {
            public DateTime ReceivedAtUtc { get; set; }
            public Dictionary<string, object> Data { get; set; }
        }
    }
}