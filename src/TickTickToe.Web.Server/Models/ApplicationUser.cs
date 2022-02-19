using Microsoft.AspNetCore.Identity;

namespace TickTickToe.Web.Server.Models;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<Game> CreatedGames { get; set; } = null!;
    public virtual ICollection<Game> InvitedGames { get; set; } = null!;
    public virtual ICollection<Game> WonGames { get; set; } = null!;

    public List<Game> Games => CreatedGames.Concat(InvitedGames)
        .OrderBy(g => g.CreatedAt)
        .ToList();
}
