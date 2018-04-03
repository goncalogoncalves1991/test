using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace WebAPI.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        [Route("")]
        public string Get()
        {
            var caller = User as ClaimsPrincipal;
            if (caller == null) return "token is not present in the cookie";
            var subjectClaim = caller.FindFirst("sub");
            if (subjectClaim != null)
            {
                return "API Received Sub: " + subjectClaim.Value + "     from:" + caller.FindFirst("client_id").Value;
            }
            else
            {                
                return "API Did not receive the Token";
            }
        }
    }
}
