using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using TickTickToe.Web.Server.Data;
using TickTickToe.Web.Server.Models;
using TickTickToe.Web.Server.Services;

namespace TickTickToe.Web.Server.Hubs;

[Authorize]
public partial class GameHub : Hub<GameHub.IGameClient>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<GameHub> _logger;
    private readonly IPasswordService _passwordService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GameHub(
        ApplicationDbContext dbContext,
        ILogger<GameHub> logger,
        IPasswordService passwordService,
        UserManager<ApplicationUser> userManager
    )
    {
        _dbContext = dbContext;
        _logger = logger;
        _passwordService = passwordService;
        _userManager = userManager;
    }

    public async Task CreateGame(string password)
    {
        // Create game
        var user = await _userManager.FindByIdAsync(Context.UserIdentifier);
        var game = new Game {PlayerOne = user};

        if (!string.IsNullOrWhiteSpace(password))
        {
            game.PasswordSalt = _passwordService.GenerateSalt();
            game.PasswordHash = _passwordService.HashPassword(Encoding.UTF8.GetBytes(password), game.PasswordSalt);
        }

        await _dbContext.Games.AddAsync(game);
        // user.CreatedGames.Add(game);

        // Add creator to game
        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
        _logger.LogInformation(game.Id.ToString());
        await Clients.Caller.AddAsPlayerOne(game.Id);
        await _dbContext.SaveChangesAsync();
    }

    public async Task JoinGame(string gameId, string? password)
    {
        var user = await _userManager.FindByIdAsync(Context.UserIdentifier);
        var game = await _dbContext.Games.FindAsync(Guid.Parse(gameId)); // Check for valid GUID
        
        // Check that the game exists
        if (game is null)
        {
            var message = new SystemMessage
            {
                Text = "That game does not exist!",
                Severity = SystemMessage.MessageSeverity.Error
            };
            await Clients.Caller.ReceiveSystemMessage(message);
            return;
        }
        
        // Stop here to explicitly load player two
        await _dbContext.Entry(game).Reference(g => g.PlayerTwo).LoadAsync();
        
        // Check player count
        if (game.PlayerTwo is not null)
        {
            var message = new SystemMessage
            {
                Text = "This game is full.",
                Severity = SystemMessage.MessageSeverity.Error
            };
            await Clients.Caller.ReceiveSystemMessage(message);
            return;
        }
        
        // Stop here to explicitly load player one
        await _dbContext.Entry(game).Reference(g => g.PlayerOne).LoadAsync();
        
        // Password checks 
        if (game.PasswordHash is not null && game.PasswordSalt is not null)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                var message = new SystemMessage
                {
                    Text = "You need to enter a password to join this game.",
                    Severity = SystemMessage.MessageSeverity.Error
                };
                await Clients.Caller.ReceiveSystemMessage(message);
                return;
            }

            var bytes = Encoding.UTF8.GetBytes(password);
            if (!_passwordService.VerifyHash(bytes, game.PasswordSalt, game.PasswordHash))
            {
                var message = new SystemMessage
                {
                    Text = "The password you entered is incorrect.",
                    Severity = SystemMessage.MessageSeverity.Error
                };
                await Clients.Caller.ReceiveSystemMessage(message);
                return;
            }
        }

        // Add player to game
        game.PlayerTwo = user;
        await Clients.User(game.PlayerOne.Id).AddPlayerTwo(user.Email);
        await Groups.AddToGroupAsync(Context.ConnectionId, game.Id.ToString());
        await Clients.Caller.AddAsPlayerTwo(game.PlayerOne.Email, game.Id);
        await _dbContext.SaveChangesAsync();
    }
}