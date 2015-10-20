using System;
using System.Collections.Generic;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.Entities
{
    internal class VoucherEntity : IEntity<Guid>
    {
        public List<Guid> CollectedVoucherIds { get; set; }

        public VoucherEntity()
        {
            CollectedVoucherIds = new List<Guid>();
        }

        #region IEntity<Guid> Members

        public Guid Id { get; set; }

        #endregion
    }
}