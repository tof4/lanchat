using System.Globalization;
using System.Linq;
using Lanchat.ClientCore;
using Lanchat.Core.Network;
using Lanchat.Terminal.Properties;
using Lanchat.Terminal.UserInterface;

namespace Lanchat.Terminal.Commands.General
{
    public class Help : ICommand
    {
        public string[] Aliases { get; } =
        {
            "help",
            "h"
        };
        public int ArgsCount => 0;
        public int ContextArgsCount => ArgsCount;

        public void Execute(string[] args)
        {
            Writer.WriteStatus("User manual");
            if (args.Length < 1)
            {
                Writer.WriteText(string.Format(Resources.Help, Storage.ConfigPath));
            }
            else
            {
                var command = Program.Commands.FirstOrDefault(x => x.Aliases.Contains(args[0]));
                if (command == null)
                {
                    Writer.WriteError(Resources.InvalidCommand);
                    return;
                }

                var commandHelp = Resources.ResourceManager.GetString($"Help_{command.Aliases[0]}", CultureInfo.CurrentCulture);

                var aliases = string.Join(", ", command.Aliases);
                Writer.WriteText($"Aliases");
                Writer.WriteText($"    {aliases}");
                if (commandHelp != null)
                {
                    Writer.WriteText(commandHelp);
                }
                Writer.WriteText("");
            }
        }

        public void Execute(string[] args, INode node)
        {
            Execute(args);
        }
    }
}