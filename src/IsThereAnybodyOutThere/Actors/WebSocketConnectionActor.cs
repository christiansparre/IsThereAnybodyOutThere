using System;
using System.Collections.Generic;
using Akka.Actor;
using Fleck;
using IsThereAnybodyOutThere.Messages;
using Newtonsoft.Json.Linq;

namespace IsThereAnybodyOutThere.Actors
{
    public class WebSocketConnectionActor : ReceiveActor
    {
        private readonly IWebSocketConnection _connection;
        private readonly IActorRef _clientRegistry;
        private string _clientId;

        public WebSocketConnectionActor(IWebSocketConnection connection, IActorRef clientRegistry)
        {
            _connection = connection;
            if (!_connection.ConnectionInfo.Cookies.ContainsKey("ClientId"))
            {
                throw new Exception("ClientId missing");
            }

            _clientId = _connection.ConnectionInfo.Cookies["ClientId"];
            _clientRegistry = clientRegistry;

            Receive<WebSocketInputMessage>(m => MessageReceived(m));
            Receive<WebSocketOpened>(m => WebSocketOpened(m));
            Receive<WebSocketClosed>(m => WebSocketClosed(m));
        }

        private void MessageReceived(WebSocketInputMessage message)
        {
            var token = JToken.Parse(message.Message);


            if (token.Value<string>("Type") == "ClientInfo")
            {
                var clientInfo = token["Payload"].ToObject<Dictionary<string, object>>();
                _clientRegistry.Tell(new ClientInfoReceived(_clientId, DateTime.UtcNow, clientInfo));
            }

            if (token.Value<string>("Type") == "Heartbeat")
            {
                var heartbeat = token["Payload"].ToObject<Dictionary<string, object>>();
                _clientRegistry.Tell(new ClientHeartbeatReceieved(_clientId, DateTime.UtcNow, heartbeat));
            }
        }


        private void WebSocketOpened(WebSocketOpened message)
        {
            _clientRegistry.Tell(new ClientConnected(_clientId));
        }


        private void WebSocketClosed(WebSocketClosed message)
        {
            _clientRegistry.Tell(new ClientDisconnected(_clientId));
            Self.Tell(PoisonPill.Instance);
        }
    }

}