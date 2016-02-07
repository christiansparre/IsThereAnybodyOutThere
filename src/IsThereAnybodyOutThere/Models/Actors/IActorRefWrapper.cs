using Akka.Actor;

namespace IsThereAnybodyOutThere.Models.Actors
{
    public interface IActorRefWrapper
    {
        IActorRef ActorRef { get; }
    }
}