using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using Newtonsoft.Json.Linq;
using Services.Models;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.ErrorResponses;
using WebAPI.Factories.Links;
using WebAPI.Factories.States;
using WebAPI.Factories.States.CollectionStates;
using WebAPI.Factories.States.SingleStates;
using WebAPI.Models.Collections;
using WebAPI.Models.Singles;

namespace WebAPI.Controllers
{
    [RoutePrefix("communities")]
    [Authorize]
    public class CommunitiesController : ApiController
    {
        [Route("")]
        public async Task<HttpResponseMessage> Get()
        {
            IStateFactory<community, CommunitiesCollectionState> _stateFactory = new CommunitiesCollectionFactory(new CommunityLinkFactory(Request));
            var instance =  CommunityService.GetInstance();

            var communities = await instance.GetAllAsync();
            if (communities.Success)
            {
                var res = communities.Result.Select<community, CommunitiesCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { communities = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, new ServiceUnvailable(Request.RequestUri, communities.Message), "application/problem+json");
            
        }
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            IStateFactory<community, CommunitySingleState> _stateFactory = new CommunitySingleFactory(Request);
            var instance = CommunityService.GetInstance();

            var community = await instance.GetByIdAsync(id);
            if(community.Success)
                return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(community.Result), "application/json");

            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, community.Message), "application/problem+json");
        }

        [Route("~/mycommunities")]
        public async Task<HttpResponseMessage> GetFromUser(string type = "member")
        {
            var id = (User as ClaimsPrincipal).FindFirst("sub").Value;
            return await GetUserCommunities(id, type);
        }

        [Route("~/users/{id}/communities")]
        public async Task<HttpResponseMessage> GetFromUser(string id, string type = "member")
        {
            return await GetUserCommunities(id, type);
            
        }       
               

        [Route("")]
        public async Task<HttpResponseMessage> Post([FromBody]  object body)
        {
            throw new NotImplementedException();

        }

        [Route("{id:int}/social")]
        public async Task<HttpResponseMessage> PostSocialNetwork([FromBody]  SocialNet body,int id)
        {            
            var adminId = GetToken();
            var instance = CommunityService.GetInstance();
            var accessToken = await getLongLiveToken(body.Token, body);
            if (adminId == null) return Request.CreateResponse(HttpStatusCode.Forbidden, new Forbidden(Request.RequestUri, "token not present in authorization header or not valid"), "application/problem+json");
            if (body == null) return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The body must not be null"), "application/problem+json");

            var res = await instance.InsertSocialNetwork(new CreateCommunitySocial { adminId = adminId,social=body.Social,token= accessToken, url=body.Url,communityId=id  });

            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { }, "application/json");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");
            }
        }


        [Route("search")]
        public async Task<HttpResponseMessage> PostSearch([FromBody]  CommunitySearch body)
        {
            var instance = CommunityService.GetInstance();
            IStateFactory<community, CommunitiesCollectionState> _stateFactory = new CommunitiesCollectionFactory(new CommunityLinkFactory(Request));

            if (body.isEmpty()) return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The body must not be null"), "application/problem+json");

            var communities = await instance.Search(body.location,body.name, body.tags);

            if (communities.Success)
            {
                var res = communities.Result.Select<community, CommunitiesCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, res, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, communities.Message), "application/problem+json");
        }
        
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> Put([FromBody]  object body)
        {
            throw new NotImplementedException();

        }

        [Route("{id:int}/user")]
        [HttpPut]
        public async Task<HttpResponseMessage> PutUser([FromBody] InputModel body, int id)
        {
            IStateFactory<community, CommunitiesCollectionState> _stateFactory = new CommunitiesCollectionFactory(new CommunityLinkFactory(Request));
            var instance = CommunityService.GetInstance();

            var token = GetToken();
            if (token == null) return Request.CreateResponse(HttpStatusCode.Forbidden, new Forbidden(Request.RequestUri, "token not present in authorization header or not valid"), "application/problem+json");
            if (body==null || body.Type == null) return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The body must have a type with value:'member' or 'admin'"), "application/problem+json");
            string type = body.Type;
            if (type == "member") { 
                var res = await instance.InsertMember(id,token);
                if (res.Success){
                    return Request.CreateResponse(HttpStatusCode.OK, new { }, "application/json");
                }
                else{
                    return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");
                }
            }
            else if(type == "organizer")
            {
                var res = await instance.InsertAdmin(id, GetToken(), body.UserId);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The body must have a type with value:'member' or 'admin'"), "application/problem+json");
        }

        public string GetToken()
        {
            var caller = User as ClaimsPrincipal;
            if (caller == null) return null;
            var subjectClaim = caller.FindFirst("sub");
            if (subjectClaim != null)
            {
                return subjectClaim.Value;
            }
            else
            {
                return null;
            }
        }

        public class InputModel
        {
            public string Type { get; set; }
            public string UserId { get; set; }

        }

        private async Task<HttpResponseMessage> GetUserCommunities(string id, string type)
        {
            IStateFactory<community, CommunitiesCollectionState> _stateFactory = new CommunitiesCollectionFactory(new CommunityLinkFactory(Request));
            var instance = CommunityService.GetInstance();

            OperationResult<IEnumerable<community>> communities = null;
            if (type == "member")
            {
                communities = await instance.GetCommunitiesFromUserByRole(id, Services.Models.Roles.Role.Member);
            }
            else if (type == "admin")
            {
                communities = await instance.GetCommunitiesFromUserByRole(id, Services.Models.Roles.Role.Admin);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The values of query string must be 'member' or 'admin"), "application/problem+json");

            if (communities.Success)
            {
                var res = communities.Result.Select<community, CommunitiesCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { communities = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, communities.Message), "application/problem+json");
        }

        private async Task<string> getLongLiveToken(string token, SocialNet body)
        {
            //facebook request GET /oauth/access_token?grant_type = fb_exchange_token & amp;client_id ={ app - id}&amp;client_secret ={ app - secret}&amp;fb_exchange_token ={ short-lived - token}
            if (body.Social == "Twitter") return token;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.facebook.com");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));

                HttpResponseMessage response = await client.GetAsync("/oauth/access_token?grant_type=fb_exchange_token&amp&client_id=1738062506473012&amp&client_secret=5e6aeb8f92541b00963bdf257c6cdd5c&amp&fb_exchange_token="+token);
                if (response.IsSuccessStatusCode)
                {
                    string product = await response.Content.ReadAsStringAsync();
                    var res = HttpUtility.ParseQueryString(product);
                    return res["access_token"];
                }
            }
            return "";
        }

        public class SocialNet
        {
            public string Url;
            public string Social;
            public string Token;           
        }

        public class CommunitySearch
        {
            public string[] tags;
            public LocationCoordinates location;
            public string name;

            public bool isEmpty()
            {
                return tags == null && name == null && location == null ;
            }

        }
    }
}
