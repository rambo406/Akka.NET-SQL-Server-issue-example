using System;
using Akka.Actor;
using Akka.NET_PlayGround.ActorCore;
using Akka.NET_PlayGround.ActorCore.Interfaces;
using Akka.NET_PlayGround.Commands;
using Akka.NET_PlayGround.Entities;
using Akka.NET_PlayGround.Events;

namespace Akka.NET_PlayGround.Actors.Aggregates
{
    internal class Voucher : AggregateRoot<VoucherEntity>
    {
        public Voucher(Guid id)
            : base(string.Format("voucher-{0}", id.ToString("N")))
        {
        }

        protected override void UpdateState(IEvent domainEvent, IActorRef sender)
        {
            domainEvent.Match()
                .With<VoucherCollectedEvent>(VoucherCollectedHandler)
                .With<VoucherCreateEvent>(VoucherCreatedHandler);
        }

        protected override bool OnCommand(object message)
        {
            return message.Match()
                .With<CreateVoucherCommand>(command =>
                {
                    Persist(new VoucherCreateEvent(command.VoucherId));

                    Console.WriteLine("Voucher id: {0}, created.", command.VoucherId);
                })
                .With<CollectVoucherCommand>(command =>
                {
                    var voucherCollectedEvent = new VoucherCollectedEvent(command.Id);

                    Persist(voucherCollectedEvent);

                    Console.WriteLine("Collect voucher id: {0}", command.Id);
                })
                .WasHandled;
        }

        private void VoucherCreatedHandler(VoucherCreateEvent obj)
        {
            State = new VoucherEntity();
        }

        private void VoucherCollectedHandler(VoucherCollectedEvent @event)
        {
            State.CollectedVoucherIds.Add(@event.Id);
        }
    }
}