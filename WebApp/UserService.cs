using IdentityServer3.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityServer3.Core.Models;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;

namespace WebApp
{
    public class UserService : IUserService
    {
        private readonly OwinEnvironmentService _oes;

        public UserService(OwinEnvironmentService oes)
        {
            _oes = oes;
        }

        public Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            throw new NotImplementedException();
        }

        public Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            throw new NotImplementedException();
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }

        public Task PostAuthenticateAsync(PostAuthenticationContext context)
        {
            return Task.FromResult(0);
        }

        public async Task PreAuthenticateAsync(PreAuthenticationContext context)
        {
            var res = await new OwinContext(_oes.Environment).Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie);
            if(res == null)
            {
                throw new InvalidOperationException("Should be authenticated at this stage");
            }
            var id = res.Identity.GetUserId();
            var name = res.Identity.GetUserName();
            context.AuthenticateResult = new AuthenticateResult(id, name);
        }

        public Task SignOutAsync(SignOutContext context)
        {
            new OwinContext(_oes.Environment).Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            
            return Task.FromResult(0);
        }
    }
}