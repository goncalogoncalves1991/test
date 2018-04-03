using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Other
{
    public class FacebookService
    {
        public static async Task<OperationResult<bool>> sendFeed(string pageId, string msg, string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.facebook.com");
                var content = "{message:'" + msg + "', access_token:'"+ token + "'}" ;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsync("/"+ pageId + "/feed", new StringContent(
                                                                                                   content.ToString(),
                                                                                                    Encoding.UTF8,
                                                                                                    "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    return new OperationResult<bool>() { Success = true, Result = true };
                }

                return new OperationResult<bool>() { Success = false, Result = false };
            }
        }
    }
}
