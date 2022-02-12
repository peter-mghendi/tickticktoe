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

        AnsiConsole.Live(render)
            .Start(ctx => 
            {
                ctx.Refresh();
                
                Thread.Sleep(TimeSpan.FromSeconds(1));
                board.Set(Row.Top, Column.Start, Value.Cross);
                render = new Markup(board.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                board.Set(Row.Top, Column.End, Value.Nought);
                render = new Markup(board.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                board.Set(Row.Middle, Column.Centre, Value.Cross);
                render = new Markup(board.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                board.Set(Row.Bottom, Column.Start, Value.Nought);
                render = new Markup(board.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                board.Set(Row.Bottom, Column.End, Value.Cross);
                render = new Markup(board.Render()).Centered();
                ctx.UpdateTarget(render);
            });

        return 0;
    }
}
