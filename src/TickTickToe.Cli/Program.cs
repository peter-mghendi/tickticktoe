using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
     config.PropagateExceptions();
    config.AddCommand<PlayCommand>("play");
    config.AddCommand<StatsCommand>("stats");
});

return app.Run(args);