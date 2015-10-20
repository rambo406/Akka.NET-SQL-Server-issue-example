using System;
using Akka.Actor;
using Akka.NET_PlayGround.ActorCore;
using Akka.NET_PlayGround.Actors.Aggregates;
using Akka.NET_PlayGround.Events;

namespace Akka.NET_PlayGround.Coordinators
{
    internal class AccountantCoordinator : AggregateCoordinator
    {
        public AccountantCoordinator()
            : base("accountant")
        {
        }

        public override Props GetProps(Guid id)
        {
            return Props.Create(() => new Accountant(id));
        }

        protected override bool Receive(object message)
        {
            var handled = base.Receive(message);
            if (!handled)
            {
                var trackVoucherCommand = message as TrackVoucherCommand;

                if (trackVoucherCommand != null)
                    ForwardCommand(trackVoucherCommand.VoucherId, trackVoucherCommand);
                else
                {
                    switch (message.ToString())
                    {
                        default:
                            return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}