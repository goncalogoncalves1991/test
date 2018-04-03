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
    public class NoticeSingleFactory : IStateFactory<notice, NoticeSingleState>
    {
        private readonly NoticeLinkFactory _links;
        private readonly HttpRequestMessage _request;

        public NoticeSingleFactory(HttpRequestMessage request)
        {
            _links = new NoticeLinkFactory(request);
            _request = request;

        }
        public NoticeSingleState Create(notice model)
        {
            var notice = new NoticeSingleState {
                id = model.id,
                title = model.title,
                description = model.description,
                date = model.initialDate.Value,
                community = new CommunitiesCollectionFactory(new CommunityLinkFactory(_request)).Create(model.community),
                _links = new NoticeSingleState.Link()
            };
            notice._links.self = _links.Self(model.id);
            notice._links.community = _links.Community(model.communityId.Value);
            return notice;
        }
    }
}
