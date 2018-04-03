using DataAccess.Models.DTOs;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        [Route("")]
        public async Task<HttpResponseMessage> Get()
        {
            IStateFactory<userInfo, UsersCollectionState> _stateFactory = new UsersCollectionFactory(new UserLinkFactory(Request));
            var instance = UserService.GetInstance();

            var users = await instance.GetAllAsync();
            if (users.Success)
            {
                var res = users.Result.Select<userInfo, UsersCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { users = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, new ServiceUnvailable(Request.RequestUri, users.Message), "application/problem+json");
   
        }

        [Route("{id}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Get(string id)
        {
            IStateFactory<userInfo, UserSingleState> _stateFactory = new UserSingleFactory(Request);
            var instance = UserService.GetInstance();

            var user = await instance.GetByIdAsync(id);
            if (user.Success)
                return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(user.Result), "application/json");

            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, user.Message), "application/problem+json");
        }

        [Route("~/communities/{id:int}/users")]
        public async Task<HttpResponseMessage> GetFromCommunity(int id, string type = "member")
        {
            IStateFactory<userInfo, UsersCollectionState> _stateFactory = new UsersCollectionFactory(new UserLinkFactory(Request));
            var instance = UserService.GetInstance();

            OperationResult<IEnumerable<userInfo>> users = null;
            if (type == "member")
            {
                users = await instance.GetUserFromCommunityByRole(id,Services.Models.Roles.Role.Member );
            }
            else if (type == "admin")
            {
                users = await instance.GetUserFromCommunityByRole(id, Services.Models.Roles.Role.Admin);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The values of query string must be 'member' or 'admin"), "application/problem+json");

            if (users.Success)
            {
                var res = users.Result.Select<userInfo, UsersCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { users = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, users.Message), "application/problem+json");
        }

        [Route("~/events/{id:int}/subscribers")]
        public async Task<HttpResponseMessage> GetFromEvent(int id, string isCheckedIn = "false")
        {
            IStateFactory<userInfo, UsersCollectionState> _stateFactory = new UsersCollectionFactory(new UserLinkFactory(Request));
            var instance = UserService.GetInstance();

            OperationResult<IEnumerable<userInfo>> users = null;
            if (isCheckedIn == "false")
            {
                users = await instance.GetUsersSubscribedOnEvent(id, Services.Services.UserService.Check_in.False);
            }
            else if (isCheckedIn == "true")
            {
                users = await instance.GetUsersSubscribedOnEvent(id, Services.Services.UserService.Check_in.True);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The values of query string must be 'true' or 'false"), "application/problem+json");

            if (users.Success)
            {
                var res = users.Result.Select<userInfo, UsersCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { users = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, users.Message), "application/problem+json");
        }

        /* [Route("")]
        public async Task<HttpResponseMessage> Post([FromBody]  object body)
        {
            throw new NotImplementedException();

        }

         [Route("")]
         public async Task<HttpResponseMessage> Put([FromBody]  object body)
         {
             throw new NotImplementedException();

         }*/
    }
}
