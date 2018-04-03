using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Factories.Links;
using WebAPI.Models.Collections;

namespace WebAPI.Factories.States.CollectionStates
{
    public class CommentsCollectionFactory : IStateFactory<comment, CommentsCollectionState>
    {
        private readonly CommentLinkFactory _links;
        private readonly HttpRequestMessage _request;

        public CommentsCollectionFactory(HttpRequestMessage request)
        {
            _links = new CommentLinkFactory(request);
            _request = request;
        }
        public CommentsCollectionState Create(comment model)
        {
            var comment = new CommentsCollectionState 
            { 
                id = model.id,
                message = model.message,
                date = model.initialDate.Value,
                userName = model.userInfo.name,
                userId = model.userInfo.id,
                //user = new UsersCollectionFactory(new UserLinkFactory(_request)).Create(model.userInfo),
                _links = new LinkCollection()
            };
            comment._links.self = _links.Self(model.id);
            return comment;
        }
    }
}
