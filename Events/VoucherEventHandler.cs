using System;
using Akka.Actor;
using Akka.NET_PlayGround.ActorCore.Interfaces;
using Akka.NET_PlayGround.Coordinators;

namespace Akka.NET_PlayGround.Events
{
    public class VoucherEventHandler : ReceiveActor
    {
        public VoucherEventHandler()
        {
            Context.System.EventStream.Subscribe(Self, typeof (VoucherCreateEvent));

            Receive(typeof (VoucherCreateEvent), VoucherCreatedEventHandler);
        }

        private bool VoucherCreatedEventHandler(object message)
        {
            var voucherCreateEvent = message as VoucherCreateEvent;

            Context.ActorOf<AccountantCoordinator>().Tell(new TrackVoucherCommand(voucherCreateEvent.VoucherId));

            return true;
        }
    }

    internal class TrackVoucherCommand : ICommand
    {
        public Guid VoucherId { get; private set; }

        public TrackVoucherCommand(Guid voucherId)
        {
            VoucherId = voucherId;
        }
    }
}