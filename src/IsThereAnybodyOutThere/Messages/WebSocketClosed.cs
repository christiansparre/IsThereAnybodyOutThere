namespace IsThereAnybodyOutThere.Messages
{
    public class WebSocketClosed
    {
        private readonly string _message = "";

        public WebSocketClosed()
        {

        }

        public WebSocketClosed(string message)
        {
            _message = message;
        }
    }
}