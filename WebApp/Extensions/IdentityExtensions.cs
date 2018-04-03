using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace WebApp.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetPhoto(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Photo");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
        
        public static string GetFullName(this IIdentity identity)
        {
            var name = ((ClaimsIdentity)identity).FindFirst("Name");
            return (name != null) ? name.Value : string.Empty;
        }
        
    }
}