using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TickTickToe.Cli.Extensions;
using TickTickToe.Core;
using Grid = TickTickToe.Core.Grid;

namespace TickTickToe.Cli.Commands;

public class PlayCommand : Command<PlayCommand.Settings>
{
    public sealed class Settings : CommandSettings {}

    public override int Execute([NotNull]CommandContext context, [NotNull]Settings settings)
    {
        var grid = new Grid();
        var render = new Markup(grid.Render()).Centered();

        AnsiConsole.Live(render)
            .Start(ctx => 
            {
                ctx.Refresh();
                
                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.Set(Row.Top, Column.Start, CellValue.Cross);
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.Set(Row.Top, Column.End, CellValue.Nought);
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.Set(Row.Middle, Column.Center, CellValue.Cross);
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.Set(Row.Bottom, Column.Start, CellValue.Nought);
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.Set(Row.Bottom, Column.End, CellValue.Cross);
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);
            });

        return 0;
    }
}