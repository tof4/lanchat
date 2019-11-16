﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lanchat.Common.TcpLib
{
    public class Host
    {
        public void Start(int port)
        {
            // Start server
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.ReceiveTimeout = -1;
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(-1);

            while (true)
            {
                Socket client = server.Accept();
                new System.Threading.Thread(() =>
                {
                    try { Process(client); } catch (Exception ex) { Console.WriteLine("Client connection processing error: " + ex.Message); }
                }).Start();
            }

            void Process(Socket client)
            {
                OnHostEvent(new EventObject("connected", client.RemoteEndPoint.ToString()), EventArgs.Empty);

                byte[] response;
                int received;

                while (true)
                {
                    // Receive message from the server:
                    response = new byte[client.ReceiveBufferSize];
                    received = client.Receive(response);
                    if (received == 0)
                    {
                        OnHostEvent(new EventObject("disconnected", client.RemoteEndPoint.ToString()), EventArgs.Empty);
                        return;
                    }

                    List<byte> respBytesList = new List<byte>(response);
                    Console.WriteLine("Client (" + client.RemoteEndPoint + "+: " + Encoding.UTF8.GetString(respBytesList.ToArray()));
                }
            }
        }

        // Host event
        public delegate void HostEventHandler(EventObject o, EventArgs e);

        public event HostEventHandler HostEvent;

        protected virtual void OnHostEvent(EventObject o, EventArgs e)
        {
            HostEvent(o, EventArgs.Empty);
        }

        // Host event object
        public class EventObject
        {
            public EventObject(string type, string ip)
            {
                Type = type;
                Ip = ip;
            }

            public string Type { get; set; }
            public string Ip { get; set; }
        }
    }

    public class Client
    {
        private TcpClient tcpclnt;
        private NetworkStream nwStream;

        public void Connect(IPAddress ip, int port)
        {
            tcpclnt = new TcpClient(ip.ToString(), port);
            nwStream = tcpclnt.GetStream();

            OnClientEvent(new
            {
                type = "connected"
            }, EventArgs.Empty);
        }

        public void Send(string content)
        {
            byte[] bytesToSend = Encoding.UTF8.GetBytes(content);
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        // Input event
        public delegate void ClientEventHandler(object o, EventArgs e);

        public event ClientEventHandler ClientEvent;

        protected virtual void OnClientEvent(object o, EventArgs e)
        {
            ClientEvent(o, EventArgs.Empty);
        }
    }
}