using DataAccess.Models.DTOs;
using Services.Models;
using Services.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
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
    [RoutePrefix("events")]
    [Authorize]
    public class EventsController : ApiController
    {
        [Route("")]
        public async Task<HttpResponseMessage> Get()
        {
            IStateFactory<@event, EventsCollectionState> _stateFactory = new EventsCollectionFactory(new EventLinkFactory(Request));
            var instance = EventService.GetInstance();

            var events = await instance.GetAllAsync();
            if (events.Success)
            {
                var res = events.Result.Select<@event, EventsCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { events = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.ServiceUnavailable, new ServiceUnvailable(Request.RequestUri, events.Message), "application/problem+json");
        }

        [Route("{id:int}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            IStateFactory<@event, EventSingleState> _stateFactory = new EventSingleFactory(Request);
            var instance = EventService.GetInstance();

            var eve = await instance.GetByIdAsync(id);
            //Thread.Sleep(3000);
            if (eve.Success)
                return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(eve.Result), "application/json");

            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, eve.Message), "application/problem+json");
        }

        [Route("~/myevents")]
        [Authorize]
        public async Task<HttpResponseMessage> GetFromUser(string time = "all")
        {
            var id = (User as ClaimsPrincipal).FindFirst("sub").Value;
            return await GetUserEvents(id,time);
        }

        [Route("~/newevents")]
        [Authorize]
        public async Task<HttpResponseMessage> GetNewEvents(string date)
        {
            var id = (User as ClaimsPrincipal).FindFirst("sub").Value;
            
            IStateFactory<@event, EventsCollectionState> _stateFactory = new EventsCollectionFactory(new EventLinkFactory(Request));
            var instance = EventService.GetInstance();
            var events = await instance.Get_New_Events_From_Community(id,date);

            if (events.Success)
            {
                var res = events.Result.Select<@event, EventsCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { events = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, events.Message), "application/problem+json");

           // return null; await GetUserEvents(id, time);
        }

        [Route("~/eventstocome")]
        [Authorize]
        public async Task<HttpResponseMessage> GetEventsToComeFromUser()
        {
            var id = (User as ClaimsPrincipal).FindFirst("sub").Value;

            IStateFactory<@event, EventsCollectionState> _stateFactory = new EventsCollectionFactory(new EventLinkFactory(Request));
            var instance = EventService.GetInstance();
            var events = await instance.Get_Events_To_Come_From_Communities_Which_User_Belongs(id);

            if (events.Success)
            {
                var res = events.Result.Select<@event, EventsCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { events = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, events.Message), "application/problem+json");


        }

        [Route("~/users/{id}/events")]
        public async Task<HttpResponseMessage> GetFromUser(string id,string time="all")
        {
            return await GetUserEvents(id,time);
                
        }

        
        [Route("~/communities/{id:int}/events")]
        public async Task<HttpResponseMessage> GetFromCommunity(int id, string time = "all")
        {
            IStateFactory<@event, EventsCollectionState> _stateFactory = new EventsCollectionFactory(new EventLinkFactory(Request));
            var instace = EventService.GetInstance();
            OperationResult<IEnumerable<@event>> events;
            if (time == "all")
                events = await instace.GetEventsByCommunityId(id);
            else if (time == "past")
                events = await instace.GetAllEventsFromCommunityInTime(id, EventService.Time.Past);
            else if (time == "future")
                events = await instace.GetAllEventsFromCommunityInTime(id, EventService.Time.Future);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The values of query string must be 'all', 'past' or 'future'"), "application/problem+json");

            if (events.Success)
            {
                var res = events.Result.Select<@event, EventsCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { events = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, events.Message), "application/problem+json");
       
        }

        [Route("search")]
        public async Task<HttpResponseMessage> PostSearch([FromBody]  EventSearch body)
        {
            var instance = EventService.GetInstance();
            IStateFactory<@event, EventsCollectionState> _stateFactory = new EventsCollectionFactory(new EventLinkFactory(Request));

            if (body.isEmpty()) return Request.CreateResponse(HttpStatusCode.BadRequest, new BadRequest(Request.RequestUri, "The body must not be null"), "application/problem+json");

            var events = await instance.Search(body.location, body.tags);

            if (events.Success)
            {
                var res = events.Result.Select<@event, EventsCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, res, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, events.Message), "application/problem+json");
        }

        /* [Route("search")]
         public async Task<HttpResponseMessage> PostSearch([FromBody]  object body)
         {
             throw new NotImplementedException();

         }

         [Route("~/api/communities/{id:int}/events")]
         public async Task<HttpResponseMessage> Post([FromBody]  object body)
         {
             throw new NotImplementedException();

         }
         */

        [Route("{id:int}/subscribe")]
        public async Task<HttpResponseMessage> PutSubscribe(int id)
        {
            IStateFactory<@event, EventsCollectionState> _stateFactory = new EventsCollectionFactory(new EventLinkFactory(Request));
            var instance = EventService.GetInstance();

            var token = GetToken();
            if (token == null) return Request.CreateResponse(HttpStatusCode.Forbidden, new Forbidden(Request.RequestUri, "token not present in authorization header or not valid"), "application/problem+json");

            var res = await instance.InsertSubscriber(id, token);
            if (res.Success)
            {   //falta acrescentar a foto quando tiver
                return Request.CreateResponse(HttpStatusCode.OK, new{ id = res.Result.id,name = res.Result.name, lastName= res.Result.lastName }, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");
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
        ///  ----------------------------------------------------------------------//
        [Route("{id:int}/issubscribed")]
        [HttpGet]
        public async Task<HttpResponseMessage> isSubscribed(int id)
        {
            var idUser = (User as ClaimsPrincipal).FindFirst("sub").Value;
            var instance = EventService.GetInstance();
            var res = await instance.isSubscribed(id,idUser);
            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { subscribed = res.Result }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");
        }
        /// -----------------------------------------------------------------------//
        [Route("{id:int}/checkin")]
        [Authorize]
        public async Task<HttpResponseMessage> PutCheckIn([FromBody]  Secret body, int id)
        {
            var idUser = (User as ClaimsPrincipal).FindFirst("sub").Value;

            
            var instance = EventService.GetInstance();
            var res = await instance.SelfCheckIn(idUser, id, body.secret);
            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");

        }
        /*
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> PutEvent([FromBody]  object body)
        {
            throw new NotImplementedException();

        }*/

        private async Task<HttpResponseMessage> GetUserEvents(string id,string time)
        {
            IStateFactory<@event, EventsCollectionState> _stateFactory = new EventsCollectionFactory(new EventLinkFactory(Request));
            var instace = EventService.GetInstance();
            OperationResult<IEnumerable<@event>> events =  null;
            if (time == "all")
                events = await instace.GetAllEventsFromUserAsync(id, EventService.Time.All);
            else if (time == "past")
                events = await instace.GetAllEventsFromUserAsync(id, EventService.Time.Past);
            else if (time == "future")
                events = await instace.GetAllEventsFromUserAsync(id, EventService.Time.Future);
            else
                events = new OperationResult<IEnumerable<@event>> { Success = false, Message = "time parameter must be one of this: 'past', 'all' or 'future'" };

            if (events.Success)
            {
                var res = events.Result.Select<@event, EventsCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { events = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, events.Message), "application/problem+json");
        }

        public class Secret
        {
            public string secret;
        }

        public class EventSearch
        {
            public string[] tags;
            public LocationCoordinates location;

            public bool isEmpty()
            {
                return tags == null && location == null;
            }            
        }
    }
}
