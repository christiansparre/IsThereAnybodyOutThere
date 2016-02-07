using Newtonsoft.Json;

namespace IsThereAnybodyOutThere.Messages
{
    public class ClientConnected
    {
        public string ClientId { get; }

        public ClientConnected(string clientId)
        {
            ClientId = clientId;
        }

    }
}