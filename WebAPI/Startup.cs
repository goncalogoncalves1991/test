using IdentityServer3.AccessTokenValidation;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(WebAPI.Startup))]
namespace WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.InboundClaimTypeMap.Clear();

            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "https://exampleapp.com/identity",
                //Authority = "https://127.0.0.1/Identity",
                ValidationMode = ValidationMode.ValidationEndpoint,
                //ClientId = "app1",
                //ClientSecret = "secret",
                RequiredScopes = new[] { "resource", "openid", "offline_access" }
            });

            app.UseWebApi(WebApiConfig.Register());
        }
    }
}
