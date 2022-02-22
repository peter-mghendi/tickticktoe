using Microsoft.AspNetCore.Identity;

namespace TickTickToe.Web.Server.Models;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<Game> CreatedGames { get; set; } = new List<Game>();
    public virtual ICollection<Game> InvitedGames { get; set; } = new List<Game>();
    public virtual ICollection<Game> WonGames { get; set; } = new List<Game>();

    public List<Game> Games => CreatedGames.Concat(InvitedGames)
        .OrderBy(g => g.CreatedAt)
        .ToList();
}
