using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TickTickToe.Web.Server.Data;
using TickTickToe.Web.Server.Hubs;
using TickTickToe.Web.Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


string connectionString;
if (builder.Environment.IsDevelopment())
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
else
{
    var databaseUri = new Uri(builder.Configuration[(string)"DATABASE_URL"]);
    var userInfo = databaseUri.UserInfo.Split(':');
    connectionString = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = databaseUri.LocalPath.TrimStart('/'),
        SslMode = SslMode.Prefer,
        TrustServerCertificate = true
    }.ToString();
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        // options.Clients.AddNativeApp("TickTickToe.Cli", clientBuilder =>
        // {
        //     clientBuilder.WithClientId("TickTickToe.Cli");
        //     clientBuilder.WithScopes(IdentityServerConstants.StandardScopes.OpenId,
        //         IdentityServerConstants.StandardScopes.Profile, "TickTickToe.Web.ServerAPI");
        //     clientBuilder.WithoutClientSecrets();
        // });
        options.Clients.Add(new()
        {
            ClientId = "TickTickToe.Cli",

            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = {new Secret("secret".Sha256())},

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,

                "TickTickToe.Web.ServerAPI"
            },
        });
    });

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] {"application/octet-stream"});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<GameHub>("/gamehub");
app.MapFallbackToFile("index.html");

app.Run();