using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using IsThereAnybodyOutThere.Client.Actors;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace IsThereAnybodyOutThere.Client
{
    public class ImRightHereClient
    {
        private readonly string _endpoint;
        private readonly string _applicationName;
        private readonly TimeSpan _heartBeatInterval;
        private readonly Func<Dictionary<string, object>> _onSendingHearbeat;
        private ActorSystem _actorSystem;

        public ImRightHereClient(string endpoint, string applicationName, TimeSpan heartBeatInterval, Func<Dictionary<string, object>> onSendingHearbeat)
        {
            _endpoint = endpoint;
            _applicationName = applicationName;
            _heartBeatInterval = heartBeatInterval;
            _onSendingHearbeat = onSendingHearbeat;
        }


        public void Start()
        {
            if (_actorSystem != null)
            {
                return;
            }

            _actorSystem = ActorSystem.Create("ImRightHereClient");
            _actorSystem.ActorOf(Props.Create(() => new WebSocketClient(_endpoint, _applicationName, _heartBeatInterval, _onSendingHearbeat)), "client");
        }

        public void Stop()
        {
            _actorSystem.Terminate();
            _actorSystem = null;
        }
    }
}
