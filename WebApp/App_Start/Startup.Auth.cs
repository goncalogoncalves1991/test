﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using WebApp.Models;
using IdentityServer3.AccessTokenValidation;
using System.Configuration;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Facebook;

namespace WebApp
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            /**
             * Used to store state and nonce temporary and then store access token on OAuth code flow
             */
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "OAuth"
            });

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");
            /*
            app.UseTwitterAuthentication(
               consumerKey: "aasdasdas",
               consumerSecret: "asdsadasd");
            */

            /*
            app.UseFacebookAuthentication(
               appId : ConfigurationManager.AppSettings["facebookClientId"],
               appSecret : ConfigurationManager.AppSettings["facebookClientSecret"]);
             */
            var facebookOptions = new FacebookAuthenticationOptions()
                                    {
                                        AppId = ConfigurationManager.AppSettings["facebookClientId"],
                                        AppSecret = ConfigurationManager.AppSettings["facebookClientSecret"],
                                        Provider = new FacebookAuthenticationProvider()
                                        {
                                            OnAuthenticated = (context) =>
                                                {
                                                    // All data from facebook in this object. 
                                                    var rawUserObjectFromFacebookAsJson = context.User;

                                                    context.Identity.AddClaim(new Claim("AccessToken", context.AccessToken));
                                                    
                                                    return Task.FromResult(0);
                                                }
                                        }
                                    };
            facebookOptions.Scope.Add("email");
            facebookOptions.Scope.Add("public_profile");
            facebookOptions.Scope.Add("user_location");
            app.UseFacebookAuthentication(facebookOptions);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = ConfigurationManager.AppSettings["googleClientId"],
                ClientSecret = ConfigurationManager.AppSettings["googleClientSecret"],
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    OnAuthenticated = (context) =>
                    {
                        context.Identity.AddClaim(new Claim("urn:google:name", context.Identity.FindFirstValue(ClaimTypes.Name)));
                        context.Identity.AddClaim(new Claim("urn:google:email", context.Identity.FindFirstValue(ClaimTypes.Email)));
                        //This following line is need to retrieve the profile image
                        context.Identity.AddClaim(new Claim("AccessToken", context.AccessToken));

                        return Task.FromResult(0);
                    }
                }
            });          

           
        }
    }
}