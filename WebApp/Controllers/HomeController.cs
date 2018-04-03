using DataAccess.Models.Create;
using IdentityModel.Client;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using Microsoft.AspNet.Identity;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Error = false;
            return View();
        }

        public async  Task<ActionResult> Search()
        {
            
            //fazer verificação 
            CommunityService service = CommunityService.GetInstance();
            var result = await service.GetAllAsync();

            return View(result.Result);
           
        }

        [Authorize]
        public async Task<ActionResult> Ola()
        {
           
           // if (Request.Browser.IsMobileDevice)
           // {
                var principal = await Request.GetOwinContext().Authentication.AuthenticateAsync("OAuth");
            
                return Content(principal.Identity.FindFirst("access_token").Value + " " + principal.Identity.FindFirst("refresh_token").Value + " " + User.Identity.GetUserId());
           // }
            /*
            var principal = await Request.GetOwinContext().Authentication.AuthenticateAsync("OAuth");
           // return principal.Identity.FindFirst("access_token").Value;
            
            var client = new HttpClient();
            client.SetBearerToken(principal.Identity.FindFirst("access_token").Value);

            var result = await client.GetStringAsync(Constants.AspNetWebApiSampleApi + "/api/Test");

            return result;*/
            //return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshToken(string refreshToken)
        {
            TokenClient _tokenClient = new TokenClient(
                                                        Constants.TokenEndpoint,
                                                        "app1",
                                                        "secret");

            var e = _tokenClient.RequestRefreshTokenAsync(refreshToken).Result;

            if (e.IsError) throw new HttpException(403, e.Error); 

            return Json(new { access_token = e.AccessToken, refresh_token = e.RefreshToken });
        }

    }
}