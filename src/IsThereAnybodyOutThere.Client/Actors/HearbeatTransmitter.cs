using System;
using System.Collections.Generic;
using Akka.Actor;
using Newtonsoft.Json;
using WebSocketSharp;

namespace IsThereAnybodyOutThere.Client.Actors
{
    public class HearbeatTransmitter : ReceiveActor
    {
        private readonly WebSocket _socket;
        private readonly TimeSpan _heartBeatInterval;
        private readonly Func<Dictionary<string, object>> _onSendingHearbeat;

        public HearbeatTransmitter(WebSocket socket, TimeSpan heartBeatInterval, Func<Dictionary<string, object>> onSendingHearbeat)
        {
            _socket = socket;
            _heartBeatInterval = heartBeatInterval;
            _onSendingHearbeat = onSendingHearbeat;

            Receive<string>(message =>
            {
                if (socket.ReadyState == WebSocketState.Open)
                {
                    var heartbeat = onSendingHearbeat();
                    heartbeat["$$Timestamp"] = DateTime.UtcNow;

                    var jsonPayload = JsonConvert.SerializeObject(new
                    {
                        Type = "Heartbeat",
                        Payload = heartbeat
                    });

                    socket.Send(jsonPayload);

                    Context.System.Scheduler.ScheduleTellOnce(heartBeatInterval, Self, "SendHeartbeat", Self);
                }
            }, msg => msg == "SendHeartbeat");
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.Zero, Self, "SendHeartbeat", Self);

            base.PreStart();
        }
    }
}