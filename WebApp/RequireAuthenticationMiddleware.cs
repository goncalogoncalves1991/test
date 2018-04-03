using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace WebApp
{
    public class RequireAuthenticationMiddleware : OwinMiddleware
    {
        public RequireAuthenticationMiddleware(OwinMiddleware next)
            :base(next)
        {
        }
        public override async Task Invoke(IOwinContext context)
        {
            if(context.Request.Path.ToString() != "/identity/login")
            {
                await Next.Invoke(context);
                return;
            }
       
            var res = await context.Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie);
            if(res == null)
            {
                context.Response.StatusCode = 401;
            }
            else
            {
                await Next.Invoke(context);
            }
        }
    }
}