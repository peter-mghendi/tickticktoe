using System.Text.Json;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TickTickToe.Core;
using TickTickToe.Web.Server.Models;

namespace TickTickToe.Web.Server.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
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
                grid => JsonSerializer.Serialize(grid.Cells, options),
                json => new()
                {
                    Cells = JsonSerializer.Deserialize<CellValue[][]>(json, options) ?? Grid.DefaultCells
                },
                new ValueComparer<Grid>(
                    (g1, g2) => Compare(g1, g2),
                    g => g.GetHashCode(),
                    g => new() { Cells = Copy(g.Cells) }
                ));

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

    private static bool Compare(Grid? g1, Grid? g2)
    {
        if (g1 is null || g2 is null) return g1 is null;

        for (var i = 0; i < g1.Cells.Length; i++)
        {
            if (!g1.Cells[i].SequenceEqual(g2.Cells[i])) return false;
        }

        return true;
    }

    private static T[][] Copy<T>(IReadOnlyList<T[]> s)
    {
        var copy = new T[s.Count][];

        for (var i = 0; i < s.Count; i++)
        {
            var row = s[i];
            copy[i] = new T[row.Length];
            for (int j = 0; j < row.Length; j++)
            {
                copy[i][j] = row[j];
            }
        }

        return copy;
    }
}