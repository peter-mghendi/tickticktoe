using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TickTickToe.Cli.Extensions;
using TickTickToe.Core;

public class PlayCommand : Command<PlayCommand.Settings>
{
    public sealed class Settings : CommandSettings {}

    public override int Execute([NotNull]CommandContext context, [NotNull]Settings settings)
    {
        var board = new Board();
        var render = new Markup(board.Render()).Centered();

        AnsiConsole.Write(render);
        return 0;
    }
}
