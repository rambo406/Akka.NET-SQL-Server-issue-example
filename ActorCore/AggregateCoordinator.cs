using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.NET_PlayGround.ActorCore.Interfaces;

namespace Akka.NET_PlayGround.ActorCore
{
    public abstract class AggregateCoordinator : ActorBase
    {
        /// <summary>
        /// Default value of maximum number of child aggregates to be kept in memory at once.
        /// </summary>
        public const int DefaultMaxChildrenCount = 64;

        /// <summary>
        ///  Default number of children to be killed at once on kill request.
        /// </summary>
        public const int DefaultChildrenToKillCount = 32;

        private readonly ISet<IActorRef> _terminatingChildren = new HashSet<IActorRef>();
        protected readonly string ChildPrefix;
        private ICollection<PendingCommand> _pendingCommands = new List<PendingCommand>(0);

        /// <summary>
        /// Number of children to be killed at once on kill request.
        /// </summary>
        public virtual int ChildrenToKillCount
        {
            get { return DefaultChildrenToKillCount; }
        }

        /// <summary>
        /// Maximum number of child aggregates to be kept in memory at once.
        /// </summary>
        public virtual int MaxChildrenCount
        {
            get { return DefaultMaxChildrenCount; }
        }

        protected AggregateCoordinator(string childPrefix)
        {
            ChildPrefix = childPrefix;
        }

        /// <summary>
        /// Props to be used for child aggregates creation.
        /// </summary>
        public abstract Props GetProps(Guid id);

        /// <summary>
        /// Forwards a command to the child aggregate. Recreates an aggregate if it has not been incarnated yet.
        /// </summary>
        protected void ForwardCommand(Guid id, ICommand command)
        {
            var pid = GetPersistenceId(id);
            var child = Context.Child(pid);
            if (!child.Equals(ActorRefs.Nobody))
            {
                if (_terminatingChildren.Contains(child))
                {
                    // add command to cache
                    _pendingCommands.Add(new PendingCommand(Sender, id, pid, command));
                    return;
                }
            }
            else
            {
                child = Create(id, pid);
            }

            child.Forward(command);
        }

        protected override bool Receive(object message)
        {
            var terminated = message as Terminated;
            if (terminated != null)
            {
                // if Terminated message was received, remove terminated actor from terminating children list
                _terminatingChildren.ExceptWith(new[] { terminated.ActorRef });

                // if there were pending commands waiting to be sent to terminated actor, recreate it
                var groups = _pendingCommands.GroupBy(cmd => cmd.PersistenceId == terminated.ActorRef.Path.Name).ToArray();

                if (_pendingCommands.Any())
                {
                    _pendingCommands = groups.First(x => !x.Key).ToList();
                    var commands = groups.First(x => x.Key);
                    foreach (var pendingCommand in commands)
                    {
                        var child = Recreate(pendingCommand.AggregateId, pendingCommand.PersistenceId);
                        child.Tell(pendingCommand.Command, pendingCommand.Sender);
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves an actor with provided <paramref name="id"/>. Respawns actor if necessary.
        /// </summary>
        protected IActorRef Retrieve(Guid id)
        {
            return Recreate(id, GetPersistenceId(id));
        }

        /// <summary>
        /// Generates a persistence id for an entity with provided aggregate identifier.
        /// </summary>
        protected string GetPersistenceId(Guid id)
        {
            return ChildPrefix + "-" + id.ToString("N");
        }

        /// <summary>
        /// Gets an child actor identified by <paramref name="pid"/> or creates it.
        /// </summary>
        private IActorRef Recreate(Guid id, string pid)
        {
            return Context.Child(pid) ?? Create(id, pid);
        }

        private IActorRef Create(Guid id, string pid)
        {
            HarvestChildren();

            var aggregateRef = Context.ActorOf(GetProps(id), pid);
            Context.Watch(aggregateRef);
            return aggregateRef;
        }

        /// <summary>
        /// Checks if children count doesn't expanded over specified boundaries
        /// and removes overflowing ones from the buffer.
        /// </summary>
        private void HarvestChildren()
        {
            var children = Context.GetChildren().ToArray();
            if (children.Length - _terminatingChildren.Count >= MaxChildrenCount)
            {
                // get all non-terminating children, kill the [ChildrenToKillCount] of them
                // and put onto terminating children list
                var notTerminating = children.Except(_terminatingChildren);
                var childrenToKill = notTerminating.Take(ChildrenToKillCount);
                foreach (var childRef in childrenToKill)
                {
                    childRef.Tell(Kill.Instance);
                    _terminatingChildren.Add(childRef);
                }
            }
        }

        #region Nested type: PendingCommand

        protected sealed class PendingCommand
        {
            public readonly Guid AggregateId;
            public readonly string PersistenceId;
            public readonly IActorRef Sender;
            public ICommand Command;

            public PendingCommand(IActorRef sender, Guid aggregateId, string persistenceId, ICommand command)
            {
                Sender = sender;
                AggregateId = aggregateId;
                Command = command;
                PersistenceId = persistenceId;
            }
        }

        #endregion
    }
}