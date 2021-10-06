using System;
using Lanchat.Core.Network;
using Lanchat.Terminal.Properties;
using Lanchat.Terminal.UserInterface;

namespace Lanchat.Terminal.Commands.FileTransfer
{
    public class Cancel : ICommand
    {
        public string Alias => "cancel";
        public int ArgsCount => 1;
        public int ContextArgsCount => 0;

        public void Execute(string[] args)
        {
            var node = Program.Network.Nodes.Find(x => x.User.ShortId == args[0]);
            if (node == null)
            {
                Writer.WriteError(Resources._UserNotFound);
                return;
            }

            Execute(args, node);
        }

        public void Execute(string[] args, INode node)
        {
            try
            {
                node.FileSender.CancelSend();
                TabsManager.FileTransfersView.GetFileTransferStatus(node).Update(Resources._FileTransferCancelled);
            }
            catch (InvalidOperationException)
            {
                Writer.WriteError(Resources._NoFileReceiveRequest);
            }
        }
    }
}