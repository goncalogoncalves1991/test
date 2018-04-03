using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Factories.Links;
using WebAPI.Factories.States.CollectionStates;
using WebAPI.Models.Collections;
using WebAPI.Models.Singles;

namespace WebAPI.Factories.States.SingleStates
{
    public class CommunitySingleFactory : IStateFactory<community, CommunitySingleState>
    {
        private readonly CommunityLinkFactory _links;
        private readonly EventsCollectionFactory eventFactory;
        private readonly NoticesCollectionFactory noticeFactory;
        private readonly UsersCollectionFactory userFactory;
        private readonly CommentsCollectionFactory commentFactory;

        public CommunitySingleFactory(HttpRequestMessage request)
        {
            _links = new CommunityLinkFactory(request);
            eventFactory = new EventsCollectionFactory(new EventLinkFactory(request));
            noticeFactory = new NoticesCollectionFactory(new NoticeLinkFactory(request));
            userFactory = new UsersCollectionFactory(new UserLinkFactory(request));
            commentFactory = new CommentsCollectionFactory(request);

        }
        public CommunitySingleState Create(community model)
        {
            var community = new CommunitySingleState
            {
                id = model.id,
                name = model.name,
                local = model.local,
                description = model.description,
                avatar = model.avatar,
                foundationDate = model.foundationDate.Value,

                events = model.@event.Select<@event, EventsCollectionState>(i => eventFactory.Create(i)),
                notices = model.notice.Select<notice, NoticesCollectionState>(i => noticeFactory.Create(i)),
                admins = model.admins.Select<userInfo, UsersCollectionState>(i => userFactory.Create(i)),
                members = model.members.Select<userInfo, UsersCollectionState>(i => userFactory.Create(i)),
                comments = model.comment.Select<comment, CommentsCollectionState>(i =>commentFactory.Create(i)),
                tags = model.tag.Select<tag,string>(i => i.name),
                _Links = new CommunitySingleState.Link()

            };

            //Add hyperMedia
            community._Links.Self = _links.Self(model.id);
            community._Links.Events = _links.Events(model.id);
            community._Links.PastEvents = _links.PastEvents(model.id);
            community._Links.FutureEvents = _links.FutureEvents(model.id);
            community._Links.Members = _links.Members(model.id);
            community._Links.Admins = _links.Admins(model.id);
            community._Links.Notices = _links.Notices(model.id);
            community._Links.Comments = _links.Comments(model.id);
            
            
            return community;
        }
    }
}
