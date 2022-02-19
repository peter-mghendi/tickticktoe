using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TickTickToe.Web.Server.Hubs;

[Authorize]
public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILogger<GameHub> logger) => _logger = logger;
    
    public void CreateRoom(string? password)
    {
        _logger.LogInformation(Context.UserIdentifier!);
    }
}