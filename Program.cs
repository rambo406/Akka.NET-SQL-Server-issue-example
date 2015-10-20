using System;
using Akka.Actor;
using Akka.NET_PlayGround.Commands;
using Akka.NET_PlayGround.Coordinators;
using Akka.NET_PlayGround.Events;
using Akka.NET_PlayGround.Indexs;
using Akka.NET_PlayGround.Views;
using Akka.Persistence.SqlServer;

namespace Akka.NET_PlayGround
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var system = ActorSystem.Create("VoucherSystem");

            SqlServerPersistence.Init(system);

            var voucherId = Guid.NewGuid();
            var voucherCoordinator = system.ActorOf<VoucherCoordinator>();
            var voucherIndex = system.ActorOf(Props.Create(() => new VoucherIndex()));
            var voucherView = system.ActorOf(Props.Create(() => new VoucherStatisticView(voucherId)), "voucher-view");
            
            system.ActorOf(Props.Create(() => new VoucherEventHandler()), "voucher-event-handler");

            voucherCoordinator.Tell(new CreateVoucherCommand(voucherId));
            voucherCoordinator.Tell(new CollectVoucherCommand(voucherId, DateTime.UtcNow));

            //            var collectedVouchers = voucherIndex.Ask<List<Guid>>(new VoucherStatisticView.GetCollectedVoucherCount(voucherId)).Result;

            //            foreach (var voucher in collectedVouchers)
            //            {
            //                Console.WriteLine("Voucher collected id: {0}", voucher);
            //            }

            //            var voucherStatisticActor = system.ActorOf(Props.Create<VoucherStatisticView>(voucherId));
            //            var totalCollectedVoucher = voucherStatisticActor.Ask<int>(new VoucherStatisticView.GetCollectedVoucherCount()).Result;

            //            Console.WriteLine("Total collected voucher: {0}", totalCollectedVoucher);

            Console.ReadLine();
        }
    }
}