# IsThereAnybodyOutThere

This is an experiement creating a simple website that lists connected (and disconnected) "clients". The web site is ASP.NET Core 1 (the framework formerly known as ASP.NET 5) and  and the client library is regular .NET 4.6.1

It's basically a WebSocket client and server where the client sends heartbeats and the server builds a list of those clients. It uses the websocket connection to track clients.

When using the client library you specify an ```ApplicationName``` that is used with ```MachineName``` and ```Username``` to provide lists on the web site. In addition there en a ```onSendingHeartbeat``` hook that allows you to provide a custom ```Dictionary<string, object>``` to send with the heartbeat.

It is very crude an not tested that much :)


