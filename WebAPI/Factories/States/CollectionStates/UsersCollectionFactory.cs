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
    public class UsersCollectionFactory : IStateFactory<userInfo, UsersCollectionState>
    {
        private readonly UserLinkFactory _links;

        public UsersCollectionFactory(UserLinkFactory links)
        {
            _links = links;
        }
        public UsersCollectionState Create(userInfo model)
        {
            var user = new UsersCollectionState 
            {
                id = model.id,
                name = model.name,
                lastName = model.lastName,
                email = model.email,
                registerDate = model.registerDate.Value,
                local = model.local,
                picture = model.picture,
                _Links = new LinkCollection()
            };

            user._Links.self = _links.Self(model.id);
            return user;
        }
    }
}
