using System;
using System.Net.Sockets;
using Akka.Actor;
using Fleck;
using IsThereAnybodyOutThere.Messages;
using Microsoft.Extensions.Configuration;

namespace IsThereAnybodyOutThere.Actors
{
    public class WebSocketServerActor : ReceiveActor
    {
        private readonly IActorRef _clientRegistry;
        private readonly IConfiguration _config;
        private WebSocketServer _webSocketServer;

        public WebSocketServerActor(IActorRef clientRegistry, IConfiguration config)
        {
            _clientRegistry = clientRegistry;
            _config = config;
            Receive((Action<IWebSocketConnection>)Handle);
        }

        private void Handle(IWebSocketConnection socket)
        {
            Sender.Tell(Context.ActorOf(Props.Create(() => new WebSocketConnectionActor(socket, _clientRegistry))));
        }

        protected override void PreStart()
        {
            _webSocketServer = new WebSocketServer(_config["WebSocketAddress"]);

            var self = Self;
            _webSocketServer.Start(socket =>
            {
                var actorRef = self.Ask<IActorRef>(socket).Result;

                socket.OnOpen = () =>
                {
                    actorRef.Tell(new WebSocketOpened());
                };

                socket.OnClose = () =>
                {
                    actorRef.Tell(new WebSocketClosed());
                };

                socket.OnMessage = message =>
                {
                    actorRef.Tell(new WebSocketInputMessage(message));
                };
                socket.OnError = exception =>
                {
                    var socketException = exception.InnerException as SocketException;
                    if (socketException?.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        actorRef.Tell(new WebSocketClosed(socketException.Message));
                    }
                    else
                    {
                        Console.WriteLine("Unknown error: " + exception.Message);
                        actorRef.Tell(new WebSocketClosed(exception.Message));
                    }
                };
            });
        }

        protected override void PostStop()
        {
            _webSocketServer?.Dispose();
        }

    }

}