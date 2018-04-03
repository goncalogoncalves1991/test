using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApp.Models;
using IdentityModel.Client;
using IdentityServer3.Core.Extensions;
using System.Collections.Generic;
using System.Net.Http;
using DataAccess.Models.Create;
using Services.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using WebApp.Extensions;

namespace WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
       
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

       
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "OAuth");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

       

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
           
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { 
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name + " "+ model.LastName ,
                    //Photo = "https://s3-eu-west-1.amazonaws.com/eventcommit/Default/images.jpg"
                };
                
                var result = await UserManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    Services.Services.UserService service = Services.Services.UserService.GetInstance();
                    var id = user.Id;
                    OperationResult<string> op = await service.PostUserAsync(new CreateUser{
                                                                                        name=model.Name,
                                                                                        email=model.Email,
                                                                                        registerDate=DateTime.Now,
                                                                                        id = user.Id,
                                                                                        lastName=model.LastName,
                                                                                        profilePicture=model.Picture != null?model.Picture.InputStream:null
                                                                                        

                    });
                    
                    if (op.Success )
                    {
                        user.Photo = op.Result;
                        var update = await UserManager.UpdateAsync(user);
                        if (update.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return RedirectToAction("Index", "OAuth");
                        }
                        await service.DeleteUser(new CreateUser { id = id });
                    }
                    await UserManager.DeleteAsync(user);
                    return RedirectToAction("Register", "Account", new { id = user.Id, message = op.Message });
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    // return RedirectToAction("Index", "Home");
                    
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            
            // Sign in the user with this external login provider if the user already has a login

            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "OAuth");//returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    return await ExternalLogin();
                    // If the user does not have an account, then prompt the user to create an account
                    //ViewBag.ReturnUrl = returnUrl;
                   // ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                   // return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email});
            }
        }
        
        private async Task<ActionResult> ExternalLogin()
        {
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Index", "Manage");
            }
            
            var accessToken = info.ExternalIdentity.Claims.FirstOrDefault(elem => elem.Type == "AccessToken").Value;
            var resources = GetResources(accessToken, info.Login.LoginProvider);
            
            var user = new ApplicationUser { UserName = resources.email, Email = resources.email, Name = resources.name, Photo= resources.picture };
            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                Services.Services.UserService service = Services.Services.UserService.GetInstance();
                var id = user.Id;
                OperationResult<string> op = await service.PostUserAsync(new CreateUser
                {
                    name = resources.first_name,
                    lastName = resources.last_name,
                    email = resources.email,
                    registerDate = DateTime.Now,
                    id = user.Id,
                    picture= resources.picture,
                    facebook = info.Login.LoginProvider =="Facebook"? resources.link:null,
                    googleplus = info.Login.LoginProvider =="Google"?resources.link:null,
                    local = resources.local 
                });
                if (!op.Success)
                {
                    await UserManager.DeleteAsync(user);
                    return RedirectToAction("Register", "Account", new { id = user.Id, message = op.Message });
                }

                result = await UserManager.AddLoginAsync(user.Id, info.Login);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "OAuth");
                }
            }
            AddErrors(result);
            return RedirectToAction("Index", "Home");
        }

        private ExternalResource GetResources(string accessToken, string providerName)
        {
            using (var client = new HttpClient())
            {
                if (providerName == "Facebook")
                {
                    return GetFacebookResources(accessToken, client);
                }
                else if (providerName == "Google")
                {
                    return GetGoogleResources(accessToken, client);
                }

                return null;
            }
        }
       
        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                string name = info.ExternalIdentity.Claims.FirstOrDefault(elem => elem.Type == ClaimTypes.Name).Value;
               // var accessToken = info.ExternalIdentity.Claims.FirstOrDefault(elem => elem.Type == "AccessToken").Value;
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Name = name };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    //var profileImageLink = GetProfileImageLink(accessToken,info.Login.ProviderKey);
                    Services.Services.UserService service = Services.Services.UserService.GetInstance();
                    var id = user.Id;
                    OperationResult<string> op = await service.PostUserAsync(new CreateUser
                    {
                        name = name,
                        email = model.Email,
                        registerDate = DateTime.Now,
                        id = user.Id
                    });
                    if (!op.Success)
                    {
                        await UserManager.DeleteAsync(user);
                        return RedirectToAction("Register", "Account", new { id = user.Id, message = op.Message });
                    }

                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToAction("Index", "OAuth");
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        private string GetProfileImageLink(string accessToken,string userId)
        {
           using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://graph.facebook.com");
                    
                    // New code:
                    HttpResponseMessage response = client.GetAsync(userId + "/picture?access_token=" + accessToken).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return response.RequestMessage.RequestUri.AbsoluteUri;
                    }
                }
           return null;
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            return LogoutHelper();
        }
        /*
         
             */

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Logout(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.WebAPPAddress);
               
                HttpResponseMessage response = await client.GetAsync(Constants.AccessTokenValidationPathEndpoint + "?token="+accessToken);
                if (response.IsSuccessStatusCode)
                {
                    return LogoutHelper();
                }
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private ActionResult LogoutHelper()
        {
            if (Session["AccessTokenTwitterSecret"] != null && Session["AccessTokenTwitterValue"] != null) {
                Session["AccessTokenTwitterSecret"] = null;
                Session["AccessTokenTwitterValue"] = null;
            } 
            var principal = Request.GetOwinContext().Authentication.AuthenticateAsync("OAuth").Result.Identity.FindFirst("id_token").Value;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Request.GetOwinContext().Authentication.SignOut("OAuth");
            return Redirect(Constants.LogoutEndpoint + "?id_token_hint=" + principal + "&post_logout_redirect_uri=" + Constants.LogoutURI);
        }

        private static ExternalResource GetGoogleResources(string accessToken, HttpClient client)
        {
            client.BaseAddress = new Uri("https://www.googleapis.com");

            // New code:
            HttpResponseMessage response = client.GetAsync("oauth2/v2/userinfo?access_token=" + accessToken).Result;
            if (response.IsSuccessStatusCode)
            {
                Stream stream = response.Content.ReadAsStreamAsync().Result;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GoogleResource));
                GoogleResource profile = (GoogleResource)ser.ReadObject(stream);
                var res = new ExternalResource { email = profile.email, first_name = profile.given_name, last_name = profile.family_name, picture = profile.picture, name = profile.name, id = profile.id, link= profile.link };
                return res;
            }
            return null;
        }

        private static ExternalResource GetFacebookResources(string accessToken, HttpClient client)
        {
            client.BaseAddress = new Uri("https://graph.facebook.com");

            HttpResponseMessage response = client.GetAsync("me?fields=email,link,location,first_name,last_name,name&access_token=" + accessToken).Result;
            if (response.IsSuccessStatusCode)
            {
                Stream stream = response.Content.ReadAsStreamAsync().Result;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ExternalResource));
                ExternalResource profile = (ExternalResource)ser.ReadObject(stream);
                profile.local = profile.location.name;
                HttpResponseMessage response2 = client.GetAsync("me/picture?access_token=" + accessToken).Result;
                if (response2.IsSuccessStatusCode)
                {
                    profile.picture = response2.RequestMessage.RequestUri.AbsoluteUri;
                }
                return profile;
            }
            return null;
        }

        [DataContract]
        public class ExternalResource
        {
            [DataMember]
            public string email { get;  set; }
            [DataMember]
            public string id { get;  set; }
            [DataMember]
            public string first_name { get;  set; }
            [DataMember]
            public string last_name { get;  set; }
            [DataMember]
            public string name { get;  set; }
            [DataMember]
            public Location location { get; set; }
            [DataMember]
            public string link { get; set; }

            public string local { get; set; }

            public string picture { get;  set; }

            [DataContract]
            public class Location
            {
                [DataMember]
                public string id { get; set; }
                [DataMember]
                public string name { get; set; }
            }
        }


    
        [DataContract]
        public class GoogleResource
        {
            [DataMember]
            public string email { get; set; }
            [DataMember]
            public string family_name { get; set; }
            [DataMember]
            public string given_name { get; set; }
            [DataMember]
            public string id { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public string picture { get; set; }
            [DataMember]
            public string link { get; set; }
        }
        #endregion
    }
}