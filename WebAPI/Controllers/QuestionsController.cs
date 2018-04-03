using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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
    [RoutePrefix("questions")]
    [Authorize]
    public class QuestionsController : ApiController
    {

        [Route("~/sessions/{id:int}/questions")]
        public async Task<HttpResponseMessage> GetFromSession(int id)
        {
            IStateFactory<question, QuestionsCollectionState> _stateFactory = new QuestionsCollectionFactory(new QuestionLinkFactory(Request));
            var instace = QuestionService.GetInstance();
            var question = await instace.GetQuestionsFromSession(id);

            if (question.Success)
            {
                var res = question.Result.Select<question, QuestionsCollectionState>(i => _stateFactory.Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { questions = res }, "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, question.Message), "application/problem+json");
        }

        [Route("{id:int}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            IStateFactory<question, QuestionSingleState> _stateFactory = new QuestionSingleFactory(Request);
            var instance = QuestionService.GetInstance();

            var question = await instance.GetByIdAsync(id);
            if (question.Success)
                return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(question.Result), "application/json");

            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, question.Message), "application/problem+json");
        }

        [Route("~/sessions/{id:int}/questions")]
        public async Task<HttpResponseMessage> Post(int id,[FromBody] QuestionBody body)
        {
            var instance = QuestionService.GetInstance();

            var token = GetToken();
            if (token == null) return Request.CreateResponse(HttpStatusCode.Forbidden, new Forbidden(Request.RequestUri, "token not present in authorization header or not valid"), "application/problem+json");

            var res = await instance.PostQuestionAsync(new CreateQuestion{authorId=token,sessionId=id,message=body.question});
            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new {id=res.Result }, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");
        }

       

        [Route("{id:int}/like")]
        public async Task<HttpResponseMessage> PostLike(int id)
        {
            var instance = QuestionService.GetInstance();

            var token = GetToken();
            if (token == null) return Request.CreateResponse(HttpStatusCode.Forbidden, new Forbidden(Request.RequestUri, "token not present in authorization header or not valid"), "application/problem+json");

            var res = await instance.LikeQuestion(new CreateQuestion { liked_user = token, id = id });
            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new {}, "application/json");
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

        public class QuestionBody
        {
            public string question;
        }


        /*
         [Route("{id:int}")]
        public async Task<HttpResponseMessage> Put(int id, [FromBody]  object body)
        {
            throw new NotImplementedException();

        }*/
    }
}
