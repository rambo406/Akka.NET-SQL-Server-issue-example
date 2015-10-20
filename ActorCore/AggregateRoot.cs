using System;
using Akka.Actor;
using Akka.NET_PlayGround.ActorCore.Interfaces;
using Akka.Persistence;

namespace Akka.NET_PlayGround.ActorCore
{
    public abstract class AggregateRoot<TEntity> : PersistentActor where TEntity : class, IEntity<Guid>
    {
        public const int MaxEventsToSnapshot = 10000;
        private readonly string _persistenceId;
        private int _eventsSinceLastSnapshot;
        protected TEntity State;

        public override string PersistenceId
        {
            get { return _persistenceId; }
        }

        protected AggregateRoot(string persistenceId)
        {
            _persistenceId = persistenceId;
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message is SnapshotOffer)
            {
                var offeredState = ((SnapshotOffer)message).Snapshot as TEntity;
                if (offeredState != null) State = offeredState;
            }
            else if (message is IEvent)
            {
                UpdateState(message as IEvent, null);
            }
            else return false;

            return true;
        }

        protected abstract void UpdateState(IEvent domainEvent, IActorRef sender);

        protected override bool ReceiveCommand(object message)
        {
            //Context.IncrementMessagesReceived();

            if (message is AggregateRootCommands.GetState)
            {
                Sender.Tell(State, Self);
                return true;
            }

            return OnCommand(message);
        }

        protected abstract bool OnCommand(object message);

        protected void Persist(IEvent domainEvent, IActorRef sender = null)
        {
            Persist(domainEvent, e =>
            {
                UpdateState(e, sender);

                Publish(e);

                if ((_eventsSinceLastSnapshot++) >= MaxEventsToSnapshot)
                {
                    SaveSnapshot(State);
                    _eventsSinceLastSnapshot = 0;
                }
            });
        }

        protected void Publish(IEvent domainEvent)
        {
            Context.System.EventStream.Publish(domainEvent);
        }
    }
}