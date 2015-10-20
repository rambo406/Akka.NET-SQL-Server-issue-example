using System;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.Entities
{
    public class AccountantEntity : IEntity<Guid>
    {
        #region IEntity<Guid> Members

        public Guid Id { get; set; }

        #endregion
    }
}