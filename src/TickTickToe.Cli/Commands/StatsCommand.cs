using System.Diagnostics.CodeAnalysis;
using Spectre.Console.Cli;

public class StatsCommand : Command<StatsCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
    }

    public override int Execute([NotNull]CommandContext context, [NotNull]Settings settings)
    {
        
        return 0;
    }
}
