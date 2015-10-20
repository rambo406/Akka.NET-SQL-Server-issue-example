using System;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.Events
{
    internal class TrackVoucherHandled : IEvent
    {
        public Guid VoucherId { get; private set; }

        public TrackVoucherHandled(Guid voucherId)
        {
            VoucherId = voucherId;

        }
    }
}