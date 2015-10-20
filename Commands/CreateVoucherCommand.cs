using System;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.Commands
{
    internal class CreateVoucherCommand : ICommand
    {
        public Guid VoucherId { get; private set; }

        public CreateVoucherCommand(Guid voucherId)
        {
            VoucherId = voucherId;
        }
    }
}