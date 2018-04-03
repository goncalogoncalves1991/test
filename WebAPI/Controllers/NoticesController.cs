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
    [RoutePrefix("notices")]
    public class NoticesController : ApiController
    {
        [Route("{id:int}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            IStateFactory<notice, NoticeSingleState> _stateFactory = new NoticeSingleFactory(Request);
            var instance = NoticeService.GetInstance();

            var notice = await instance.GetByIdAsync(id);
            if (notice.Success)
                return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(notice.Result), "application/json");

            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, notice.Message), "application/problem+json");
        }

        [Route("~/communities/{id:int}/notices")]
        public async Task<HttpResponseMessage> GetFromCommunity(int id)
        {
            IStateFactory<notice, NoticesCollectionState> _stateFactory = new NoticesCollectionFactory(new NoticeLinkFactory(Request));
            var instace = NoticeService.GetInstance();
            var notices = await instace.GetNoticesFromCommunity(id);

            if (notices.Success)
            {
                var res = notices.Result.Select<notice, NoticesCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { notices = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, notices.Message), "application/problem+json");

        }

         /*[Route("~/api/communities/{id:int}/notices")]
         public async Task<HttpResponseMessage> Post(int id, [FromBody]  object body)
         {
             throw new NotImplementedException();

         }*/

    }
}
