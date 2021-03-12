using System;
using Lanchat.Core.System;

namespace Lanchat.Tests.Mock
{
    public class NodeState : INodeState
    {
        public bool Ready { get; } = true;
        public Guid Id { get; } = Guid.NewGuid();
    }
}