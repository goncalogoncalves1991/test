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
using WebAPI.Factories.States;
using WebAPI.Factories.States.CollectionStates;
using WebAPI.Factories.States.SingleStates;
using WebAPI.Models.Collections;
using WebAPI.Models.Singles;

namespace WebAPI.Controllers
{
    [RoutePrefix("comments")]
    [Authorize]
    public class CommentsController : ApiController
    {

        [Route("{id:int}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            IStateFactory<comment, CommentSingleState> _stateFactory = new CommentSingleFactory(Request);
            var instance = CommentService.GetInstance();
            
            var comment = await instance.GetCommentByIdAsync(id);
            if (comment.Success)
                return Request.CreateResponse(HttpStatusCode.OK, _stateFactory.Create(comment.Result), "application/json");

            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, comment.Message), "application/problem+json");
        }

        [Route("~/events/{idEvents:int}/comments")]
        public async Task<HttpResponseMessage> GetCommentsFromEvent(int idEvents)
        {
            IStateFactory<comment, CommentSingleState> _stateFactory = new CommentSingleFactory(Request);
            var instance = CommentService.GetInstance();

            var comments = await instance.GetAllCommentsFromEvent(idEvents);
            if (comments.Success)
            {
                var res = comments.Result.Select<comment,CommentsCollectionState>(i => new CommentsCollectionFactory(Request).Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new {comments = res}, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, comments.Message), "application/problem+json");
        }

        [Route("~/communities/{idCommunity:int}/comments")]
        public async Task<HttpResponseMessage> GetCommentsFromCommunity(int idCommunity)
        {
            IStateFactory<comment, CommentSingleState> _stateFactory = new CommentSingleFactory(Request);
            var instance = CommentService.GetInstance();

            var comments = await instance.GetAllCommentsFromCommunity(idCommunity);
            if (comments.Success)
            {
                var res = comments.Result.Select<comment, CommentsCollectionState>(i => new CommentsCollectionFactory(Request).Create(i));
                return Request.CreateResponse(HttpStatusCode.OK, new { comments = res }, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.NotFound, new NotFound(Request.RequestUri, comments.Message), "application/problem+json");
        }

         
        [Route("~/communities/{idCommunity:int}/comments")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostCommentOnCommunity(int idCommunity,[FromBody]  CommentFromUser body)
        {
            var instance = CommentService.GetInstance();
            var token = GetToken();
            var res = await instance.PostCommentOnCommunity(new CreateComment { communityId = idCommunity, message = body.comment, authorId = token });

            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, res.Result, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");
        }
        

        [Route("~/events/{idEvents:int}/comments")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostCommentOnEvent(int idEvents, [FromBody]  CommentFromUser body)
         {
            //IStateFactory<@event, EventsCollectionState> _stateFactory = new EventsCollectionFactory(new EventLinkFactory(Request));
            var instance = CommentService.GetInstance();
            var token = GetToken();
            
            var res = await instance.PostCommentOnEvent(new CreateComment{ eventId=idEvents,message=body.comment,authorId=token });

            if (res.Success)
            {
                return Request.CreateResponse(HttpStatusCode.OK, res.Result, "application/json");
            }

            return Request.CreateResponse(HttpStatusCode.Conflict, new Conflict(Request.RequestUri, res.Message), "application/problem+json");

        }

        public class CommentFromUser
        {
            public string comment;
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
