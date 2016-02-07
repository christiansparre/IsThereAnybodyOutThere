# IsThereAnybodyOutThere

This is an experiment creating a simple website that lists connected (and disconnected) "clients". The web site is ASP.NET Core 1 (the framework formerly known as ASP.NET 5) and  and the client library is regular .NET 4.6.1

It's basically a WebSocket client and server where the client sends heartbeats and the server builds a list of those clients. It uses the websocket connection to track clients.

When using the client library you specify an ```ApplicationName``` that is used with ```MachineName``` and ```Username``` to provide lists on the web site. In addition there en a ```onSendingHeartbeat``` hook that allows you to provide a custom ```Dictionary<string, object>``` to send with the heartbeat.

It is very crude an not tested that much :)

## Why?

At work we have a number of WPF and WinForms apps and in one of them there is a tiny feature that is not use very much, maybe every couple of months. And there's little bug where the backend service occasionally needs to be restarted for this one little button to work. Yeah I know that bug should be fixed, but, you know... priorities :) 

Anyway when this button needs pressing, we keep saying how nice it would be to know if there are actually anyone using the app. So, you know, we can restart the service without too much trouble.

So why not use the opportunity and a weekend to experiment a bit with some new stuff.
