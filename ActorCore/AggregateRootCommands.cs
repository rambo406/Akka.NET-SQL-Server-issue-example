using System;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.ActorCore
{
    public class AggregateRootCommands
    {
        #region Nested type: GetState

        public sealed class GetState : ICommand
        {
            public readonly Guid Id;

            public GetState(Guid id)
            {
                Id = id;
            }
        }

        #endregion
    }
}