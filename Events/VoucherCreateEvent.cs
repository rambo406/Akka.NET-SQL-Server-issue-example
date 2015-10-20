using System;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.Events
{
    internal class VoucherCreateEvent : IEvent
    {
        public Guid VoucherId { get; private set; }

        public VoucherCreateEvent(Guid voucherId)
        {
            VoucherId = voucherId;
        }
    }
}