using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Factories.Links;
using WebAPI.Models.Singles;

namespace WebAPI.Factories.States.SingleStates
{
    public class UserSingleFactory : IStateFactory<userInfo,UserSingleState>
    {
        private readonly HttpRequestMessage _request;
        private readonly UserLinkFactory _links;

        public UserSingleFactory(HttpRequestMessage Request)
        {
            _links = new UserLinkFactory(Request);
            _request = Request;
        }

        public UserSingleState Create(userInfo model)
        {
            var user = new UserSingleState
            {
                id = model.id,
                name = model.name,
                lastName = model.lastName,
                email = model.email,
                registerDate = model.registerDate.Value,
                local = model.local,
                picture= model.picture,
                gitHub = model.github,
                facebook= model.facebook,
                twitter = model.twitter,
                biography = model.biography,
                linkedin= model.linkedin,
                
                _links = new UserSingleState.Link()
            };

            user._links.self = _links.Self(model.id);
            user._links.subscribedEvents = _links.events(model.id);
            user._links.admin = _links.CommunitiesWhereUserIsAdmin(model.id);
            user._links.member = _links.CommunitiesWhereUserIsMember(model.id);
            return user;
        }
    }
}
