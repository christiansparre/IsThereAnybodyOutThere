using WebSocketSharp;

namespace IsThereAnybodyOutThere.Client.Messages
{
    public class MessageReceived
    {
        public MessageEventArgs Args { get; }

        public MessageReceived(MessageEventArgs args)
        {
            Args = args;
        }
    }
}