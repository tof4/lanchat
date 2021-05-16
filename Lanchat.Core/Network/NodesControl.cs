using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Lanchat.Core.Config;
using Lanchat.Core.Network.Models;
using Lanchat.Core.Tcp;

namespace Lanchat.Core.Network
{
    internal class NodesControl
    {
        private readonly IConfig config;
        private readonly IContainer container;

        internal NodesControl(IConfig config, IContainer container)
        {
            this.config = config;
            this.container = container;
            Nodes = new List<INode>();
        }

        internal List<INode> Nodes { get; }
        internal event EventHandler<INode> NodeCreated;

        internal Node CreateNode(IHost host)
        {
            var scope = container.BeginLifetimeScope(b => { b.RegisterInstance(host).As<IHost>(); });
            var node = scope.Resolve<Node>();
            Nodes.Add(node);
            node.Connected += OnConnected;
            node.CannotConnect += (sender, args) =>
            {
                CloseNode(sender, args);
                scope.Dispose();
            };

            node.Disconnected += (sender, args) =>
            {
                CloseNode(sender, args);
                scope.Dispose();
            };
            NodeCreated?.Invoke(this, node);
            node.Connection.Initialize();
            return node;
        }

        private void CloseNode(object sender, EventArgs e)
        {
            var node = (Node) sender;
            var id = node.Id;
            Nodes.Remove(node);
            node.Connected -= OnConnected;
            node.CannotConnect -= CloseNode;
            node.Disconnected -= CloseNode;
            Trace.WriteLine($"Node {id} disposed");
        }

        private void OnConnected(object sender, EventArgs e)
        {
            var node = (Node) sender;
            var nodesList = new NodesList();
            nodesList.AddRange(Nodes
                .Where(x => x.Id != node.Id)
                .Select(x => x.Host.Endpoint.Address));
            node.Output.SendData(nodesList);

            if (!config.SavedAddresses.Contains(node.Host.Endpoint.Address))
            {
                config.SavedAddresses.Add(node.Host.Endpoint.Address);
            }
        }
    }
}