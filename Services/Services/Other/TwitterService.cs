
using Spring.Social.OAuth1;
using Spring.Social.Twitter.Api;
using Spring.Social.Twitter.Connect;
using System;
using System.Threading.Tasks;

namespace Services.Services.Other
{
    public class TwitterService
    {
        private const string TwitterConsumerKey = "VExXFVYY3UPW5DEXi0A2wY1fj";
        private const string TwitterConsumerSecret = "qOEx6HdHrOV8E2wNwunmqJgSNYDYxHhya1seeVJsbz5TnZ9c1S";

        static IOAuth1ServiceProvider<ITwitter> twitterProvider =
            new TwitterServiceProvider(TwitterConsumerKey, TwitterConsumerSecret);

        public static async Task<OperationResult<bool>> sendTweet(string msg, string token)
        {
            char[] delimiterChars = { ':' };
            
            string[] words = token.Split(delimiterChars);

            //OAuthToken token2 = new OAuthToken(words[1],words[0]);
            ITwitter twitterClient = twitterProvider.GetApi(words[1], words[0]);

            try
            {
                var res = await twitterClient.TimelineOperations.UpdateStatusAsync(msg);
                return new OperationResult<bool>() { Result = true, Success = true };
            }
            catch (Exception e)
            {
                return new OperationResult<bool>() { Success = false, Message=e.InnerException.Message };
            }
        }
    }
}
