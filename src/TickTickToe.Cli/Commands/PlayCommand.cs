using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;
using TickTickToe.Cli.Extensions;
using TickTickToe.Core;
using Grid = TickTickToe.Core.Grid;

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
                grid.TopStart = CellValue.Cross;
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.TopEnd = CellValue.Nought;
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.MiddleCenter = CellValue.Cross;
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.BottomStart = CellValue.Nought;
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);

                Thread.Sleep(TimeSpan.FromSeconds(1));
                grid.BottomEnd = CellValue.Cross;
                render = new Markup(grid.Render()).Centered();
                ctx.UpdateTarget(render);
            });

        return 0;
    }
}
