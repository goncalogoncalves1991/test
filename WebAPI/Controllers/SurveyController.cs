using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using Services.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPI.ErrorResponses;
using WebAPI.Factories.States;
using WebAPI.Factories.States.SingleStates;
using WebAPI.Models.Singles;
using static DataAccess.Models.Create.CreateSurvey;
using static DataAccess.Models.Create.CreateSurveyAnswer;

namespace WebAPI.Controllers
{
    [Authorize]
    public class SurveyController : ApiController
    {

        [Route("~/events/{id:int}/survey")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            IStateFactory<IEnumerable<surveyQuestion>, SurveySingleState> _stateFactory = new SurveySingleFactory(Request);
            var instance = SurveyService.GetInstance();
            var token = GetToken();

            var questions = await instance.GetEventSurvey(id,token);
            if (questions.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(questions.Result), "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, questions.Message), "application/problem+json");
        }

        [Route("~/events/{id:int}/survey_answer")]
        public async Task<HttpResponseMessage> Get_survey_answer(int id)
        {
            IStateFactory<IEnumerable<surveyAnswer>, SurveyAnswerSingleState> _stateFactory = new SurveyAnswerSingleFactory(Request);
            var instance = SurveyService.GetInstance();
            var token = GetToken();

            var questions = await instance.GetSurveyResponses(id, token);
            if (questions.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(questions.Result), "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, questions.Message), "application/problem+json");
        }

        [Route("~/events/{id:int}/survey")]
        public async Task<HttpResponseMessage> Post([FromBody]  SurveyQuestion[] body, int id)
        {
            var instance = SurveyService.GetInstance();

            var token = GetToken();
            if (token == null) return Request.CreateResponse(HttpStatusCode.Forbidden, new Forbidden(Request.RequestUri, "token not present in authorization header or not valid"), "application/problem+json");

            var res = await instance.PostSurvey(new CreateSurvey { authorId = token, eventId = id, questions= body});
            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { }, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");
        }

        [Route("~/events/{id}/survey_answer")]
        public async Task<HttpResponseMessage> PostAnswer([FromBody]  SurveyQuestionAnswer[] body, int id)
        {
            var instance = SurveyService.GetInstance();

            var token = GetToken();
            if (token == null) return Request.CreateResponse(HttpStatusCode.Forbidden, new Forbidden(Request.RequestUri, "token not present in authorization header or not valid"), "application/problem+json");

            var res = await instance.PostSurveyAnswer(new CreateSurveyAnswer { authorId = token, eventId = id, questions = body });
            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { }, "application/json");
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
    }
}