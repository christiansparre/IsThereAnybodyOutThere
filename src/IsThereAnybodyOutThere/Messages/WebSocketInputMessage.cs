namespace IsThereAnybodyOutThere.Messages
{
    public class WebSocketInputMessage
    {
        public string Message { get; }

        public WebSocketInputMessage(string message)
        {
            Message = message;
        }
    }
}