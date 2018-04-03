using DataAccess.Models.DTOs;
using Services.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    [RoutePrefix("sessions")]
    public class SessionsController : ApiController
    {
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Get(int id)
        {
            IStateFactory<session, SessionSingleState> _stateFactory = new SessionSingleFactory(Request);
            var instance = SessionService.GetInstance();

            var session = await instance.GetByIdAsync(id);
            if (session.Success)
            {
                return  Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(session.Result), "application/json"); 
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, session.Message), "application/problem+json");
        }
        
        [Route("~/events/{id:int}/sessions")]
        public async Task<HttpResponseMessage> GetAll(int id)
        {
            IStateFactory<session, SessionsCollectionState> _stateFactory = new SessionsCollectionFactory(new SessionLinkFactory(Request));
            var instance = SessionService.GetInstance();

            var sessions = await instance.GetSessionsFromEvent(id);
            if (sessions.Success)
            {
                var res = sessions.Result.Select<session, SessionsCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { sessions = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, sessions.Message), "application/problem+json");    
        }


        /*[Route("~/api/events/{id:int}/sessions")]
        public async Task<HttpResponseMessage> Post(int id, [FromBody]  object body)
        {
            throw new NotImplementedException();

        }*/
    }
}
