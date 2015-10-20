using System;
using Akka.Actor;
using Akka.NET_PlayGround.ActorCore;
using Akka.NET_PlayGround.ActorCore.Interfaces;
using Akka.NET_PlayGround.Entities;
using Akka.NET_PlayGround.Events;

namespace Akka.NET_PlayGround.Actors.Aggregates
{
    public class Accountant : AggregateRoot<AccountantEntity>
    {
        public Accountant(Guid id) : base(string.Format("accountant-{0}", id.ToString("N")))
        {
        }

        protected override void UpdateState(IEvent domainEvent, IActorRef sender)
        {
            domainEvent.Match()
                .With<TrackVoucherHandled>(TrackVoucherHandledHandler);
        }

        private void TrackVoucherHandledHandler(TrackVoucherHandled @event)
        {
            State = new AccountantEntity();
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        protected override bool OnCommand(object message)
        {
            return message.Match()
                .With<TrackVoucherCommand>(TrackVoucherCommandHandler)
                .WasHandled;
        }

        private void TrackVoucherCommandHandler(TrackVoucherCommand command)
        {
            Persist(new TrackVoucherHandled(command.VoucherId), Self);
        }
    }
}