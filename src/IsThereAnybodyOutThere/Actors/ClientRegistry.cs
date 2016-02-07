using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using IsThereAnybodyOutThere.Messages;
using IsThereAnybodyOutThere.Models;
using IsThereAnybodyOutThere.Models.Clients;

namespace IsThereAnybodyOutThere.Actors
{
    public class ClientRegistry : ReceiveActor
    {
        Dictionary<string, Client> _clients = new Dictionary<string, Client>();
        private ICancelable _cleanUpTask;

        public ClientRegistry()
        {
            Receive<ClientConnected>(m => HandleClientConnected(m));
            Receive<ClientDisconnected>(m => HandleClientDisconnected(m));
            Receive<ClientHeartbeatReceieved>(m => HandleHeartbeatReceieved(m));
            Receive<ClientInfoReceived>(m => HandleClientInfoReceived(m));
            Receive<GetClients>(m => HandleGetClients(m));
            Receive<CleanupRegistry>(m => HandleCleanupRegistry(m));
        }

        private void HandleCleanupRegistry(CleanupRegistry message)
        {
            // Get instances of clients that have not been connected in 48 hours
            var oldClients = _clients.Values.Where(a => !a.Connected && a.LastHeartbeatReceivedAtUtc < DateTime.UtcNow.AddHours(48)).ToList();

            foreach (var oldClient in oldClients)
            {
                _clients.Remove(oldClient.Id);
            }

            // Cleanup all clients heartbeat lists to only includ the last 50

            foreach (var id in _clients.Keys)
            {
                UpdateClient(id, client =>
                {
                    client.Heartbeats = client.Heartbeats.Take(50).ToList();
                });
            }
        }

        private void HandleClientInfoReceived(ClientInfoReceived message)
        {
            UpdateClient(message.ClientId, client =>
            {
                client.MachineName = message.Info["$$MachineName"] as string;
                client.Username = message.Info["$$Username"] as string;
                client.ApplicationName = message.Info["$$ApplicationName"] as string;
            });
        }

        private void HandleGetClients(GetClients message)
        {
            Sender.Tell(_clients.Select(c => new ClientModel
            {
                Id = c.Value.Id,
                MachineName = c.Value.MachineName,
                Username = c.Value.Username,
                LastHeartbeatReceivedAtUtc = c.Value.LastHeartbeatReceivedAtUtc,
                Connected = c.Value.Connected,
                Heartbeats = c.Value.Heartbeats.Select(h => new ClientModel.Heartbeat { ReceivedAtUtc = h.ReceivedAtUtc, Data = h.Data }).ToList(),
                ApplicationName = c.Value.ApplicationName
            }).ToList());
        }

        private void HandleHeartbeatReceieved(ClientHeartbeatReceieved message)
        {
            UpdateClient(message.ClientId, client =>
            {
                client.LastHeartbeatReceivedAtUtc = message.ReceivedAtUtc;
                client.Heartbeats.Insert(0, new Client.Heartbeat { ReceivedAtUtc = message.ReceivedAtUtc, Data = message.Heartbeat });
            });
        }

        private void HandleClientDisconnected(ClientDisconnected message)
        {
            UpdateClient(message.ClientId, client => client.Connected = false);
        }

        private void HandleClientConnected(ClientConnected message)
        {
            UpdateClient(message.ClientId, client => client.Connected = true);
        }

        private void UpdateClient(string clientId, Action<Client> update)
        {
            Client client;
            if (!_clients.TryGetValue(clientId, out client))
            {
                _clients[clientId] = client = new Client { Id = clientId };
            }

            update(client);
        }

        protected override void PreStart()
        {
            _cleanUpTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), Self, new CleanupRegistry(), Self);
        }

        protected override void PostStop()
        {
            _cleanUpTask.Cancel();
        }

        private class Client
        {
            public string Id { get; set; }
            public bool Connected { get; set; }

            public DateTime LastHeartbeatReceivedAtUtc { get; set; }
            public string MachineName { get; set; }
            public string Username { get; set; }

            public List<Heartbeat> Heartbeats { get; set; } = new List<Heartbeat>();
            public string ApplicationName { get; set; }

            public class Heartbeat
            {
                public DateTime ReceivedAtUtc { get; set; }
                public Dictionary<string, object> Data { get; set; }
            }
        }
    }
}