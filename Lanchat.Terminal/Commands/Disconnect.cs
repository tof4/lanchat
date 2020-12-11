﻿using Lanchat.Terminal.Properties;
using Lanchat.Terminal.UserInterface;

namespace Lanchat.Terminal.Commands
{
    public static class Disconnect
    {
        public static void Execute(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Ui.Log.Add(Resources.Manual_Disconnect);
                return;
            }

            var node = Program.Network.Nodes.Find(x => x.ShortId == args[0]);
            if (node != null)
            {
                node.Disconnect();
            }
            else
            {
                Ui.Log.Add(Resources.Info_NotFound);
            }
        }
    }
}