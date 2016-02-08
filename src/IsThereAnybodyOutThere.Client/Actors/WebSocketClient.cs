using System;
using System.Collections.Generic;
using System.Diagnostics;
using Akka.Actor;
using IsThereAnybodyOutThere.Client.Messages;

namespace IsThereAnybodyOutThere.Client.Actors
{
    public class WebSocketClient : ReceiveActor
    {
        private readonly string _endpoint;
        private readonly string _applicationName;
        private readonly TimeSpan _heartBeatInterval;
        private readonly Func<Dictionary<string, object>> _onSendingHearbeat;
        private readonly Action<string, Exception> _onError;

        private int _connectCount = 0;

        public WebSocketClient(string endpoint, string applicationName, TimeSpan heartBeatInterval, Func<Dictionary<string, object>> onSendingHearbeat, Action<string, Exception> onError = null)
        {
            _endpoint = endpoint;
            _applicationName = applicationName;
            _heartBeatInterval = heartBeatInterval;
            _onSendingHearbeat = onSendingHearbeat;
            _onError = onError;

            Receive<ConnectionClosed>(closed =>
            {
                // Received connection closed from child. Try to reconnect.
                StartConnection();
            });

            Receive<ErrorOccurred>(error =>
            {
                // Received error from child. Try to reconnect.
                StartConnection();
                _onError?.Invoke(error.Message, error.Exception);
            });

            Receive<ConnectionOpened>(open =>
            {
                // Success, reset the connection count
                _connectCount = 0;
            });

            Receive<string>(msg =>
            {
                _connectCount++;
                Context.ActorOf(Props.Create(() => new WebSocketClientConnection(_endpoint, _applicationName, _heartBeatInterval, _onSendingHearbeat)), "connection+" + Guid.NewGuid()); // Generate a unique child name, previous child might still be closing down
            }, msg => msg == "CreateClientConnection");

            Trace.WriteLine("Created WebSocketClient actor");
        }

        private void StartConnection()
        {
            if (_connectCount > 5)
            {
                // Ok we have tried to connect 5 times in a row without success. Add a little time before the next

                var connectDelay = TimeSpan.FromSeconds(10);

                if (_connectCount > 10)
                {
                    // Something is not right wait even longer
                    connectDelay = TimeSpan.FromMinutes(1);
                }

                Console.WriteLine($"Connect attempts at {_connectCount}, delay next try by {connectDelay}");

                Context.System.Scheduler.ScheduleTellOnce(connectDelay, Self, "CreateClientConnection", Self);
            }
            else
            {
                Self.Tell("CreateClientConnection");
            }
        }

        protected override void PreStart()
        {
            StartConnection();
        }
    }
}