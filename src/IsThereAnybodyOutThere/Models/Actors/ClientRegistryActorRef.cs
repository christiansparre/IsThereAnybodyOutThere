using Akka.Actor;

namespace IsThereAnybodyOutThere.Models.Actors
{
    public class ClientRegistryActorRef : IActorRefWrapper
    {
        public ClientRegistryActorRef(IActorRef clientRegistry)
        {
            ActorRef = clientRegistry;
        }
        public IActorRef ActorRef { get; }
    }
}