﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Lanchat.Core.Config;
using Lanchat.Core.Network;
using NetCoreServer;

namespace Lanchat.Core.TransportLayer
{
    internal class Server : TcpServer
    {
        private readonly INodesDatabase nodesDatabase;
        private readonly NodesControl nodesControl;
        private readonly AddressChecker addressChecker;

        internal Server(IPAddress address, int port, NodesControl nodesControl, AddressChecker addressChecker, INodesDatabase nodesDatabase) : base(address, port)
        {
            this.nodesControl = nodesControl;
            this.nodesDatabase = nodesDatabase;
            this.addressChecker = addressChecker;
            OptionDualMode = true;
        }

        protected override void OnStarted()
        {
            Trace.WriteLine($"Server listening on {Endpoint.Port}");
            base.OnStarted();
        }

        protected override TcpSession CreateSession()
        {
            var session = new Session(this);
            session.Connected += SessionOnConnected;
            return session;
        }

        private void SessionOnConnected(object sender, EventArgs e)
        {
            var session = (Session)sender;
            var savedNode = nodesDatabase.SavedNodes.FirstOrDefault(x => Equals(x.IpAddress, session.Endpoint.Address));
            if (savedNode is { Blocked: true })
            {
                Trace.WriteLine($"Connection from {session.Endpoint.Address} blocked");
                session.Close();
                return;
            }

            try
            {
                nodesControl.CreateNode(session);
            }
            catch (ArgumentException)
            {
                session.Close();
            }
        }

        protected override void OnError(SocketError error)
        {
            Trace.WriteLine($"Server errored: {error}");
        }
    }
}