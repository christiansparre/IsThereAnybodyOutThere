using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Akka.Actor;
using IsThereAnybodyOutThere.Client.Messages;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace IsThereAnybodyOutThere.Client.Actors
{
    public class WebSocketClientConnection : ReceiveActor
    {
        private readonly string _endpoint;
        private readonly string _applicationName;
        private readonly TimeSpan _heartBeatInterval;
        private readonly Func<Dictionary<string, object>> _onSendingHearbeat;
        private WebSocket _socket;
        private string _clientId;

        public WebSocketClientConnection(string endpoint, string applicationName, TimeSpan heartBeatInterval, Func<Dictionary<string, object>> onSendingHearbeat)
        {
            _endpoint = endpoint;
            _applicationName = applicationName;
            _heartBeatInterval = heartBeatInterval;
            _onSendingHearbeat = onSendingHearbeat;
            _clientId = Guid.NewGuid().ToString();

            // Message handlers
            Receive<ConnectionOpened>(m => HandleConnectionOpened(m));
            Receive<ConnectionClosed>(m => HandleConnectionClosed(m));
            Receive<MessageReceived>(m => HandleMessageReceived(m));
            Receive<ErrorOccurred>(m => HandleErrorOccurred(m));
        }

        private void HandleErrorOccurred(ErrorOccurred message)
        {
            Context.Stop(Self);
            Context.Parent.Tell(message);
        }

        private void HandleMessageReceived(MessageReceived message)
        {
            Console.WriteLine($"Received message");
        }

        private void HandleConnectionClosed(ConnectionClosed message)
        {
            Console.WriteLine("Connection closed");
            Context.Stop(Self);
            Context.Parent.Tell(message);
        }

        private void HandleConnectionOpened(ConnectionOpened message)
        {
            var jsonPayload = JsonConvert.SerializeObject(new
            {
                Type = "ClientInfo",
                Payload = new Dictionary<string, object>
                {
                    ["$$MachineName"] = Environment.MachineName,
                    ["$$Username"] = Environment.UserName,
                    ["$$ApplicationName"] = _applicationName
                }
            });
            _socket.Send(jsonPayload);
            Context.ActorOf(Props.Create(() => new HearbeatTransmitter(_socket, _heartBeatInterval, _onSendingHearbeat)), "heartbeats");
            Context.Parent.Tell(message);
        }

        protected override void PreStart()
        {
            var self = Self;
            _socket = new WebSocket(_endpoint);
            _socket.OnOpen += (sender, args) =>
            {
                self.Tell(new ConnectionOpened());
            };

            _socket.OnMessage += (sender, args) =>
            {
                self.Tell(new MessageReceived(args));
            };

            _socket.OnError += (sender, args) =>
            {
                self.Tell(new ErrorOccurred(args.Message, args.Exception));
            };
            _socket.OnClose += (sender, args) =>
            {
                self.Tell(new ConnectionClosed());
            };

            _socket.SetCookie(new Cookie("ClientId", _clientId));
            _socket.ConnectAsync();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            _socket.Close();
            Console.WriteLine("Restarting");
            base.PreRestart(reason, message);
        }
    }
}