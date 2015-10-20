using System;
using Akka.Actor;
using Akka.NET_PlayGround.Events;

namespace Akka.NET_PlayGround.Indexs
{
    internal class VoucherIndex : ReceiveActor
    {
        public VoucherIndex()
        {
//            Context.System.EventStream.Subscribe(Self, typeof (VoucherCollectedEvent));

            Receive<VoucherCollectedEvent>(@event => { Console.WriteLine("Received event from voucher index"); });
        }
    }
}