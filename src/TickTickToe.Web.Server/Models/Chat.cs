namespace TickTickToe.Web.Server.Models;

public class Chat
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public virtual ApplicationUser Sender { get; set; } = null!;
    public DateTime Sent { get; } = DateTime.Now;
}