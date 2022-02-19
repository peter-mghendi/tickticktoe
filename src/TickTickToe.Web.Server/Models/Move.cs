using TickTickToe.Core;

namespace TickTickToe.Web.Server.Models;

public class Move
{
    public int Id { get; set; }
    public Row Row { get; set; }
    public Column Column { get; set; }
    public virtual ApplicationUser Player { get; set; } = null!;
    public virtual Game Game { get; set; } = null!;
}