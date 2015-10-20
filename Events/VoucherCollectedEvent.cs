using System;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.Events
{
    internal class VoucherCollectedEvent : IEvent
    {
        public Guid Id { get; private set; }

        public VoucherCollectedEvent(Guid id)
        {
            Id = id;
        }
    }
}