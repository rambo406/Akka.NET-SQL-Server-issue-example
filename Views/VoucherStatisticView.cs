using System;
using Akka.NET_PlayGround.ActorCore.Interfaces;
using Akka.NET_PlayGround.Events;
using Akka.Persistence;

namespace Akka.NET_PlayGround.Views
{
    internal class VoucherStatisticView : PersistentView
    {
        private readonly Guid _id;
        private int _voucherCollectCount;

        public override string PersistenceId
        {
            get { return String.Format("voucher-{0}", _id.ToString("N")); }
        }

        public override string ViewId
        {
            get { return String.Format("voucher-statistic-{0}", _id.ToString("N")); }
        }

        public VoucherStatisticView(Guid id)
        {
            _id = id;
        }

        protected override bool Receive(object message)
        {
            return message.Match()
                .With<VoucherCollectedEvent>(e => { RecordVoucherCollectCount(e.Id); })
                .With<GetCollectedVoucherCount>(e => { Sender.Tell(_voucherCollectCount, Self); })
                .WasHandled;
        }

        private void RecordVoucherCollectCount(Guid id)
        {
            Console.WriteLine("Noticed you collected a voucher");

            _voucherCollectCount += 1;
        }

        #region Nested type: GetCollectedVoucherCount

        public class GetCollectedVoucherCount : ICommand
        {
        }

        #endregion
    }
}