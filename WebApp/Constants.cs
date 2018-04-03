using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp
{
    public static class Constants
    {
        public const string BaseAddress = "https://exampleapp.com/identity";
        public const string WebAPPAddress = "https://exampleapp.com";
        //public const string BaseAddress = "https://127.0.0.1/identity";
        //public const string WebAPPAddress = "https://127.0.0.1";
        public const string AuthorizeEndpoint = BaseAddress + "/connect/authorize";
        public const string LogoutEndpoint = BaseAddress + "/connect/endsession";
        public const string TokenEndpoint = BaseAddress + "/connect/token";
        public const string UserInfoEndpoint = BaseAddress + "/connect/userinfo";
        public const string IdentityTokenValidationEndpoint = BaseAddress + "/connect/identitytokenvalidation";
        public const string AccessTokenValidationPathEndpoint = "identity/connect/accesstokenvalidation";
        public const string TokenRevocationEndpoint = BaseAddress + "/connect/revocation";

        public const string AspNetWebApiSampleApi = "https://exampleapp.com/api/";
        //public const string AspNetWebApiSampleApi = "http://127.0.0.1:8000";
        public static string AuthCallback= WebAPPAddress+"/OAuth/callback";
        public static string LogoutURI = WebAPPAddress+"/Home/Index";
    }
}