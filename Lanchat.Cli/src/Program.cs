﻿// Lanchat 2
// Let's all love lain

using Lanchat.Cli.ConfigLib;
using Lanchat.Cli.PromptLib;
using Lanchat.Common.NetworkLib;
using System;
using System.Diagnostics;
using System.Threading;

namespace Lanchat.Cli.Program
{
    public class Program
    {
        public bool DebugMode;
        public Network network;

        public void Main()
        {
            // Check is debug enabled
            Trace.Assert(DebugMode = true);

            // Trace listener
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            // Load or create config file
            Config.Load();

            // Show welcome screen
            Prompt.Welcome();

            // Check nickname
            if (string.IsNullOrEmpty(Config.Nickname))
            {
                // If nickname is blank create new with up to 20 characters
                var nick = Prompt.Query("Choose nickname:");
                while (nick.Length > 20 && nick.Length != 0)
                {
                    Prompt.Alert("Nick cannot be blank or longer than 20 characters");
                    nick = Prompt.Query("Choose nickname:");
                }
                Config.Nickname = nick;
            }


            // Initialize event handlers
            EventHandlers eventHandlers = new EventHandlers(this);

            // Initialize prompt
            var prompt = new Prompt();
            prompt.RecievedInput += eventHandlers.OnRecievedInput;
            new Thread(prompt.Init).Start();

            // Initialize network
            network = new Network(Config.Port, Config.Nickname);
            network.RecievedMessage += eventHandlers.OnRecievedMessage;
            network.NodeConnected += eventHandlers.OnNodeConnected;
            network.NodeDisconnected += eventHandlers.OnNodeDisconnected;
            network.ChangedNickname += eventHandlers.OnChangedNickname;
            network.Start();
        }
    }
}