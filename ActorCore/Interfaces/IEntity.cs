namespace Akka.NET_PlayGround.ActorCore.Interfaces
{
    public interface IEntity<TIdentifier>
    {
        TIdentifier Id { get; set; }
    }
}