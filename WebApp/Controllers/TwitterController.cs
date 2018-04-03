using System.Web.Mvc;

using Spring.Social.OAuth1;
using Spring.Social.Twitter.Api;
using Spring.Social.Twitter.Connect;
using System;

namespace WebApp.Controllers
{
    [Authorize]
    [RoutePrefix("twitter")]
    public class TwitterController : Controller
    {
        private const string TwitterConsumerKey = "VExXFVYY3UPW5DEXi0A2wY1fj";
        private const string TwitterConsumerSecret = "qOEx6HdHrOV8E2wNwunmqJgSNYDYxHhya1seeVJsbz5TnZ9c1S";

        IOAuth1ServiceProvider<ITwitter> twitterProvider =
            new TwitterServiceProvider(TwitterConsumerKey, TwitterConsumerSecret);

        [Route("login")]
        public ActionResult login()
        {
            OAuthToken requestToken = twitterProvider.OAuthOperations.FetchRequestTokenAsync("https://exampleapp.com/twittercallback", null).Result;
            Session["RequestToken"] = requestToken;

            return Redirect(twitterProvider.OAuthOperations.BuildAuthenticateUrl(requestToken.Value, null));
        }

        [AllowAnonymous]
        [Route("~/twittercallback")]
        public ActionResult twittercallback(string oauth_verifier)
        {
            OAuthToken requestToken = Session["RequestToken"] as OAuthToken;
            AuthorizedRequestToken authorizedRequestToken = new AuthorizedRequestToken(requestToken, oauth_verifier);
            OAuthToken token = twitterProvider.OAuthOperations.ExchangeForAccessTokenAsync(authorizedRequestToken, null).Result;

            Session["AccessTokenTwitterSecret"] = token.Secret;
            Session["AccessTokenTwitterValue"] = token.Value;

            return Redirect(Constants.WebAPPAddress);
        }
        /*
        [Route("send")]
        public ActionResult send()
        {
            OAuthToken token2 = Session["AccessToken"] as OAuthToken;
            ITwitter twitterClient = twitterProvider.GetApi(token2.Value, token2.Secret);

            try
            {
                var res = twitterClient.TimelineOperations.UpdateStatusAsync("teste").Result;
            }
            catch (Exception e)
            {
                Console.Write("");
            }
            return null;
        }*/
    }
}