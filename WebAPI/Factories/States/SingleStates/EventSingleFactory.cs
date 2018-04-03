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
    public class EventSingleFactory : IStateFactory<@event, EventSingleState>
    {
        private readonly EventLinkFactory _links;
        private readonly CommunitiesCollectionFactory communityFactory;
        private readonly UsersCollectionFactory userFactory;
        private readonly SessionsCollectionFactory sessionFactory;
        private readonly CommentsCollectionFactory commentFactory;
        private readonly SurveySingleFactory surveyFactory;

        public EventSingleFactory(HttpRequestMessage request)
        {
            _links = new EventLinkFactory(request);
            communityFactory = new CommunitiesCollectionFactory(new CommunityLinkFactory(request));
            userFactory = new UsersCollectionFactory(new UserLinkFactory(request));
            sessionFactory = new SessionsCollectionFactory(new SessionLinkFactory(request));
            commentFactory = new CommentsCollectionFactory(request);
            surveyFactory = new SurveySingleFactory(request);
        }
        public EventSingleState Create(@event model)
        {
            var eve = new EventSingleState
            {
                id = model.id,
                title = model.title,
                local = model.local,
                initDate = model.initDate,
                endDate = model.endDate,
                description = model.description,
                nrOfTickets = model.nrOfTickets.Value,
                community = communityFactory.Create(model.community),
                subscribers = model.eventSubscribers.Select<eventSubscribers, UsersCollectionState>(i => userFactory.Create(i.userInfo)),
                session = model.session.Select<session, SessionsCollectionState>(i => sessionFactory.Create(i)),
                comments = model.comment.Select<comment, CommentsCollectionState>(i => commentFactory.Create(i)),
                survey = surveyFactory.Create(model.survey),
                tag = model.tag.Select<tag,string>(i => i.name),
                _links = new EventSingleState.Link()
            };
            
            //add hypermedia
            eve._links.self = _links.Self(model.id);
            eve._links.sessions = _links.Sessions(model.id);
            eve._links.community = _links.Community(model.communityId.Value);
            eve._links.comments = _links.Comments(model.id);
            eve._links.subscribers = _links.Subscribers(model.id);
            eve._links.subscriberCheckedIn = _links.SubscribersCheckedIn(model.id);
            
            return eve;
        }
    }
}
