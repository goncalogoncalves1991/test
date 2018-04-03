using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class HelperController : Controller
    {
        [AllowAnonymous]
        public async Task<ActionResult> Refresh()
        {
            TokenClient _tokenClient = new TokenClient(
                                                        Constants.TokenEndpoint,
                                                        "app1",
                                                        "secret");
            var principal = await Request.GetOwinContext().Authentication.AuthenticateAsync("OAuth");

            var e = _tokenClient.RequestRefreshTokenAsync(principal.Identity.FindFirst("refresh_token").Value).Result;

            if (e.IsError) throw new HttpException(403, e.Error);

            var claims = new List<Claim>();
            claims.Add(principal.Identity.FindFirst("expires_at"));
            claims.Add(principal.Identity.FindFirst("id_token"));
            claims.Add(new Claim("access_token", e.AccessToken));
            claims.Add(new Claim("refresh_token", e.RefreshToken));

            Request.GetOwinContext().Authentication.SignOut("OAuth");     

            var id = new ClaimsIdentity(claims, "OAuth");
            Request.GetOwinContext().Authentication.SignIn(id);

            return Content(e.AccessToken);
        }
    }
}