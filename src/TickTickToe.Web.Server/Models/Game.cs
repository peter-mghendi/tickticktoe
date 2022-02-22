using TickTickToe.Core;

namespace TickTickToe.Web.Server.Models;

public class Game
{
    public Guid Id { get; set; }
    public Grid Grid { get; set; } = new();
    public byte[]? PasswordHash { get; set; } = null;
    public byte[]? PasswordSalt { get; set; } = null;
    public virtual ApplicationUser PlayerOne { get; set; } = null!;
    public virtual ApplicationUser? PlayerTwo { get; set; } = null;
    public virtual ApplicationUser? Winner { get; set; } = null;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public virtual IList<Move> Moves { get; set; } = new List<Move>();
}