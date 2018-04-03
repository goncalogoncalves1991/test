using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace WebApp
{
    public static class InMemoryConfig
    {
        public static IEnumerable<Client> Clients
        {
            get
            {
                return new[]
                {
                    new Client
                    {
                        ClientName = "App1",
                        ClientId = "app1",
                        Flow = Flows.AuthorizationCode,
                        ClientSecrets = new List<Secret>
                            {
                                new Secret("secret".Sha256())
                            },
                    
                        RedirectUris = new List<string>
                        {
                            Constants.AuthCallback,
                        },
                        
                        PostLogoutRedirectUris = new List<string>{
                             Constants.LogoutURI
                        },
                        // access to identity data and api1
                        AllowedScopes = new List<string>
                        {
                            "resource","openid",StandardScopes.OfflineAccess.Name
                        }, 
                        AccessTokenType = AccessTokenType.Reference,
                        AccessTokenLifetime = 10*60, // 5min 
                        RequireConsent=false
                    }
                   
                };
            }
        }

        public static List<InMemoryUser> Users
        {
            get
            {
                return new List<InMemoryUser>
                {
                    new InMemoryUser
                    {
                        Subject = "1",
                        Username = "Alice",
                        Claims = new []
                        {
                            new Claim ("email", "alice4demos@gmail.com"),
                            new Claim ("name", "Alice")
                        },
                        Enabled = true,
                        Provider="Google",
                        Password = null,
                    }
                };
            }
        }

        public static IEnumerable<Scope> Scopes
        {
            get
            {
                return StandardScopes.All.Concat(new[]
                {
                    new Scope
                    {
                        Name = "resource",
                        DisplayName = "Allow some resource access",
                        Claims = new List<ScopeClaim>
                        {
                            new ScopeClaim("email", true),
                        }
                    },
                    StandardScopes.OfflineAccess
                });
            }
        }
    }
}