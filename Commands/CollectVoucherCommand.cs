using System;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.Commands
{
    internal class CollectVoucherCommand : ICommand
    {
        public DateTime CollectDateTime { get; private set; }
        public Guid Id { get; private set; }

        public CollectVoucherCommand(Guid id, DateTime collectDateTime)
        {
            Id = id;
            CollectDateTime = collectDateTime;
        }
    }
}