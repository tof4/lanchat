using Lanchat.Core.Network;

namespace Lanchat.Terminal.Commands
{
    public interface ICommand
    {
        string Alias { get; }
        int ArgsCount { get; }
        int ContextArgsCount { get; }
        void Execute(string[] args);
        void Execute(string[] args, INode node);
    }
}