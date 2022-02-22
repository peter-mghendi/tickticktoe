using System.Text.Json;
using IdentityModel.Client;
using Spectre.Console.Cli;
using TickTickToe.Cli.Commands;

var response = await RequestTokenAsync();
Console.WriteLine("Token obtained. Press any key to continue.");

Console.ReadLine();
await CallServiceAsync(response.AccessToken);

static async Task<TokenResponse> RequestTokenAsync()
{
    var client = new HttpClient();
    
    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7067");
    if (disco.IsError) throw new Exception(disco.Error);
    
    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,

        ClientId = "TickTickToe.Cli",
        ClientSecret = "secret",
                
        Scope = "TickTickToe.Web.ServerAPI" 
    });

    if (response.IsError) throw new Exception(response.Error);
    return response;
}

static async Task CallServiceAsync(string token)
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7067")
    };

    client.SetBearerToken(token);
    var response = await client.GetStringAsync("WeatherForecast");

    var x = JsonSerializer.Deserialize<object>(response);
    var indented = JsonSerializer.Serialize(x, new JsonSerializerOptions() {WriteIndented = true});
    Console.WriteLine(indented);
}

var app = new CommandApp();
app.Configure(config =>
{
     config.PropagateExceptions();
    config.AddCommand<PlayCommand>("play");
    config.AddCommand<StatsCommand>("stats");
});

return app.Run(args);