using System;
using Akka.Actor;
using Akka.NET_PlayGround.ActorCore;
using Akka.NET_PlayGround.Actors.Aggregates;
using Akka.NET_PlayGround.Commands;

namespace Akka.NET_PlayGround.Coordinators
{
    internal class VoucherCoordinator : AggregateCoordinator
    {
        public VoucherCoordinator()
            : base("voucher")
        {
        }

        public override Props GetProps(Guid id)
        {
            return Props.Create(() => new Voucher(id));
        }

        protected override bool Receive(object message)
        {
            var handled = base.Receive(message);
            if (!handled)
            {
                var collectVoucherCommand = message as CollectVoucherCommand;
                var createVoucherCommand = message as CreateVoucherCommand;

                if (collectVoucherCommand != null)
                    ForwardCommand(collectVoucherCommand.Id, collectVoucherCommand);
                else if (createVoucherCommand != null)
                    ForwardCommand(createVoucherCommand.VoucherId, createVoucherCommand);
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