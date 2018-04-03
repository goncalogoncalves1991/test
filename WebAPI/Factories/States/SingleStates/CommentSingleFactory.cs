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
    public class CommentSingleFactory : IStateFactory<comment, CommentSingleState>
    {
        private readonly CommentLinkFactory _links;
        private readonly HttpRequestMessage _request;
        public CommentSingleFactory(HttpRequestMessage request)
        {
            _links = new CommentLinkFactory(request);
            _request = request;
            
        }
        public CommentSingleState Create(comment model)
        {
            var comment = new CommentSingleState(){
                id= model.id,
                message = model.message,
                date = model.initialDate.Value,
                user = new UsersCollectionFactory(new UserLinkFactory(_request)).Create(model.userInfo),
                _Links = new CommentSingleState.Link()
            };
            comment._Links.Self = _links.Self(model.id);
            if(model.@event != null){
                comment.@event = new EventsCollectionFactory(new EventLinkFactory(_request)).Create(model.@event);
                comment._Links.Event = _links.Event(model.eventId.Value);
            }
            else
            {
                comment.community = new CommunitiesCollectionFactory(new CommunityLinkFactory(_request)).Create(model.community);
                comment._Links.Community = _links.Community(model.communityId.Value);
            } 
            return comment;
        }
    }
}

