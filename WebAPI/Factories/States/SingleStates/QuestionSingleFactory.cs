using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Factories.Links;
using WebAPI.Factories.States.CollectionStates;
using WebAPI.Models.Singles;

namespace WebAPI.Factories.States.SingleStates
{
    public class QuestionSingleFactory : IStateFactory<question, QuestionSingleState>
    {
        private readonly HttpRequestMessage _request;
        private readonly QuestionLinkFactory _links;

        public QuestionSingleFactory(HttpRequestMessage Request)
        {
            _links = new QuestionLinkFactory(Request);
            _request = Request;
        }

        public QuestionSingleState Create(question model)
        {
            var question = new QuestionSingleState
            {
                id = model.id,
                question = model.message,
                author = new UsersCollectionFactory(new UserLinkFactory(_request)).Create(model.userInfo),
                session = new SessionsCollectionFactory(new SessionLinkFactory(_request)).Create(model.session),
                _links = new QuestionSingleState.Link()

            };
            question._links.self = _links.Self(model.id);
            question._links.session = _links.Session(model.sessionId.Value);
            question._links.author = _links.User(model.authorId);
            return question;
        }
    }
}
