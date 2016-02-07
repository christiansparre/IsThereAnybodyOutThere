namespace IsThereAnybodyOutThere.Messages
{
    public class ClientDisconnected
    {
        public string ClientId { get; }

        public ClientDisconnected(string clientId)
        {
            ClientId = clientId;
        }
    }
}