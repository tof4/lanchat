﻿namespace Lanchat.Common.NetworkLib.Api
{
    /// <summary>
    /// Network API outputs class.
    /// </summary>
    public class Methods
    {
        // Constructor
        internal Methods(Network network)
        {
            this.network = network;
        }

        // Fields
        private readonly Network network;

        /// <summary>
        /// Send message to all nodes.
        /// </summary>
        /// <param name="message">content</param>
        public void SendAll(string message)
        {
            network.NodeList.ForEach(x =>
            {
                if (x.Client != null)
                {
                    x.Client.SendMessage(message);
                }
            });
        }
    }
}