using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lanchat.Core.Models;
using Lanchat.Core.Network;
using Lanchat.Core.P2PHandlers;

namespace Lanchat.Core
{
    internal class NodesControl
    {
        internal List<Node> Nodes { get; }
        internal event EventHandler<Node> NodeCreated;
        private readonly IConfig config;
        private readonly P2P network;

        internal NodesControl(IConfig config, P2P network)
        {
            this.config = config;
            this.network = network;
            Nodes = new List<Node>();
        }
        
        internal Node CreateNode(INetworkElement networkElement)
        {
            var node = new Node(networkElement, config);
            Nodes.Add(node);
            node.Resolver.RegisterHandler(new NodesListHandler(network));
            node.Connected += OnConnected;
            node.CannotConnect += CloseNode;
            node.Disconnected += CloseNode;
            NodeCreated?.Invoke(this, node);
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
            node.Dispose();
            Trace.WriteLine($"Node {id} disposed");
        }

        private void OnConnected(object sender, EventArgs e)
        {
            var node = (Node) sender;
            var nodesList = new NodesList();
            nodesList.AddRange(Nodes.Where(x => x.Id != node.Id)
                .Select(x => x.NetworkElement.Endpoint.Address.ToString()));
            node.NetworkOutput.SendData(nodesList);

            if (!config.SavedAddresses.Contains(node.NetworkElement.Endpoint.Address))
                config.SavedAddresses.Add(node.NetworkElement.Endpoint.Address);
        }
    }
}