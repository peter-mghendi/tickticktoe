using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TickTickToe.Core;
using TickTickToe.Core.Models;
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

    public async Task JoinGame(Guid gameId, string? password)
    {
        var user = await _userManager.FindByIdAsync(Context.UserIdentifier);
        var game = await _dbContext.Games.FindAsync(gameId);
        
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

    public async Task PerformTurn(Guid gameId, Row row, Column column, CellValue value)
    {
        // We hope my logic is sound and this user and game exist
        var user = await _userManager.FindByIdAsync(Context.UserIdentifier);
        var game = (await _dbContext.Games.FindAsync(gameId))!;

        // Stop here to explicitly load players and moves 
        await _dbContext.Entry(game).Reference(g => g.PlayerOne).LoadAsync();
        await _dbContext.Entry(game).Reference(g => g.PlayerTwo).LoadAsync();
        await _dbContext.Entry(game)
            .Collection(g => g.Moves)
            .Query()
            .Include(m => m.Player)
            .LoadAsync();

        // Check turn
        if (!IsValidTurn(game, user))
        {
            var message = new SystemMessage
            {
                Text = "It is not your turn!",
                Severity = SystemMessage.MessageSeverity.Error
            };
            await Clients.Caller.ReceiveSystemMessage(message);
            return;
        }

        // Perform turn
        var move = new Move
        {
            Row = row,
            Column = column,
            Player = user,
        };

        game.Grid.Set(row, column, value);
        game.Moves.Add(move);
        await Clients.Group(gameId.ToString()).SetGridValue(row, column, value);
        
        // Game end conditions
        if (game.Grid.IsWon())
        {
            game.Winner = user;
            await Clients.Group(gameId.ToString()).EndGame(winner: user.Email);
        }
        else if (game.Grid.IsFull())
        {
            await Clients.Group(gameId.ToString()).EndGame(winner: null);
        }
        
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task LeaveRoom(Guid gameId)
    {
        var user = await _userManager.FindByIdAsync(Context.UserIdentifier);
        var game = (await _dbContext.Games.FindAsync(gameId))!;
        
        await _dbContext.Entry(game).Reference(g => g.PlayerOne).LoadAsync();
        await _dbContext.Entry(game).Reference(g => g.PlayerTwo).LoadAsync();
        await _dbContext.Entry(game).Reference(g => g.Winner).LoadAsync();
        
        // Check game status
        if (game.Winner is null)
        {
            var message = new SystemMessage
            {
                Text = $"{user.Email} has left the game.",
                Severity = SystemMessage.MessageSeverity.Info
            };
            await Clients.Caller.ReceiveSystemMessage(message);
            
            game.Winner = user.Id == game.PlayerOne.Id ? game.PlayerTwo! : game.PlayerOne;
            message = new SystemMessage
            {
                Text = $"{game.Winner.Email} wins by default.",
                Severity = SystemMessage.MessageSeverity.Info
            };
            await Clients.Caller.ReceiveSystemMessage(message);

            await Clients.OthersInGroup(gameId.ToString()).EndGame(winner: game.Winner!.Email);
        }

        // Remove player from room
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, game.Id.ToString());
        await Clients.Caller.RemoveFromGame();
        await _dbContext.SaveChangesAsync();
    }

    private static bool IsValidTurn(Game game, ApplicationUser currentUser)
    {
        if (game.Moves.Count == 0)
        {
            // Player one's turn
            if (currentUser.Id == game.PlayerTwo!.Id) return false;
            return true;
        }
        
        // HACK: Reorder moves: Find out why they're coming out of the db in random order
        var moves = game.Moves.OrderBy(g => g.Id).ToList();
        if (moves[^1].Player.Id == game.PlayerTwo!.Id)
        {
            // Player one's turn
            if (currentUser.Id == game.PlayerTwo!.Id) return false;
        }
        else
        {
            // Player two's turn
            if (currentUser.Id == game.PlayerOne.Id) return false;
        }

        return true;
    }
}