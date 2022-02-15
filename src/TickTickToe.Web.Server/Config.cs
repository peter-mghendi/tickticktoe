using Duende.IdentityServer;

namespace TickTickToe.Web.Server;

using Duende.IdentityServer.Models;

public static class Config
{
    public static IEnumerable<Client> Clients =>
        new []
        {
            // client credentials flow sample
            new Client
            {
                ClientId = "TickTickToe.Cli",

                AllowedGrantTypes = GrantTypes.Code,
                AllowOfflineAccess = true,
                ClientSecrets = { new Secret("secret".Sha256()) },

                // RedirectUris =           { "http://localhost:21402/signin-oidc" },
                // PostLogoutRedirectUris = { "http://localhost:21402/" },
                // FrontChannelLogoutUri =    "http://localhost:21402/signout-oidc",

                AllowedScopes = 
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,

                    "TickTickToe.Web.ServerAPI" 
                },
            },
        };
}