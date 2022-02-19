using System.Text.Json;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using TickTickToe.Core;
using TickTickToe.Web.Server.Models;

namespace TickTickToe.Web.Server.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public DbSet<Chat> Chats => Set<Chat>();

    public DbSet<Game> Games => Set<Game>();

    public DbSet<Move> Moves => Set<Move>();


    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

        builder.Entity<ApplicationUser>().Ignore(a => a.Games);

        builder.Entity<Game>()
            .Property(g => g.Grid)
            .HasConversion(
                grid => JsonSerializer.Serialize(grid, options),
                json => JsonSerializer.Deserialize<Grid>(json, options) ?? new()
            );

        builder.Entity<Game>()
            .HasOne(g => g.PlayerOne)
            .WithMany(a => a.CreatedGames);

        builder.Entity<Game>()
            .HasOne(g => g.PlayerTwo)
            .WithMany(a => a.InvitedGames);

        builder.Entity<Game>()
            .HasOne(g => g.Winner)
            .WithMany(a => a.WonGames);
    }
}