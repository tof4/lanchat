using System.Linq;
using System.Net;
using Lanchat.Core.Network;
using Lanchat.Terminal.Properties;
using Lanchat.Terminal.UserInterface;

namespace Lanchat.Terminal.Commands.Blocking
{
    public class Block : ICommand
    {
        public string Alias => "block";
        public int ArgsCount => 1;
        public int ContextArgsCount => 0;

        public void Execute(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                Writer.WriteError(Resources.Help_block);
                return;
            }

            if (args[0].Length == 4)
            {
                Execute(args, Program.Network.Nodes.Find(x => x.User.ShortId == args[0]));
            }

            if (IPAddress.TryParse(args[0], out var ipAddress))
            {
                var node = Program.Network.Nodes.Find(x => Equals(x.Host.Endpoint.Address, ipAddress));
                if (node == null)
                {
                    SaveBlockInConfig(ipAddress);
                }
                else
                {
                    Execute(args, node);
                }
            }
            else
            {
                Writer.WriteError(Resources._IncorrectValues);
            }
        }

        public void Execute(string[] args, INode node)
        {
            var ipAddress = node.Host.Endpoint.Address;
            node.Disconnect();
            SaveBlockInConfig(ipAddress);
            Writer.WriteText(string.Format(Resources._Blocked, ipAddress));
        }

        private static void SaveBlockInConfig(IPAddress ipAddress)
        {
            if (Program.Config.BlockedAddresses.Any(x => Equals(x, ipAddress)))
            {
                Writer.WriteError(Resources._AlreadyBlocked);
                return;
            }

            Program.Config.BlockedAddresses.Add(ipAddress);
        }
    }
}