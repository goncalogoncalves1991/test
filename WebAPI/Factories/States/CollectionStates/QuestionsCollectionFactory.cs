using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Factories.Links;
using WebAPI.Models.Collections;

namespace WebAPI.Factories.States.CollectionStates
{
    public class QuestionsCollectionFactory : IStateFactory<question, QuestionsCollectionState>
    {
        private readonly QuestionLinkFactory _links;

        public QuestionsCollectionFactory(QuestionLinkFactory links)
        {
            _links = links;
        }
        public QuestionsCollectionState Create(question model)
        {
            var question = new QuestionsCollectionState
            {
                id = model.id,
                question = model.message,
                likes = model.likes.Value,
                likers = model.user_like.Select(elem => elem.id).ToArray(),
                authorId = model.authorId,
                authorName = model.userInfo.name,
                _links = new LinkCollection()

            };
            question._links.self = _links.Self(model.id);
            return question;
        }
    }
}
