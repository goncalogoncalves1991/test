using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

[assembly: OwinStartupAttribute(typeof(WebApp.Startup))]
namespace WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            app.Use(typeof(RequireAuthenticationMiddleware));
            app.Map("/identity", idSrvApp =>
            {
                var factory = new IdentityServerServiceFactory()
                        .UseInMemoryClients(InMemoryConfig.Clients)
                        .UseInMemoryScopes(InMemoryConfig.Scopes);

                factory.UserService = new Registration<IUserService>(typeof(UserService));
                
                idSrvApp.UseIdentityServer(new IdentityServerOptions
                {                     
                    SiteName = "EventCommit IdentityServer3",
                    SigningCertificate = LoadCertificate(),
                    Factory = factory,

                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true,                        
                        EnableSignOutPrompt = false,
                    },
                });

            });
        }

        public static X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}
