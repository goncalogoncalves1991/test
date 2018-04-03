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
    public class CommunitiesCollectionFactory : IStateFactory<community, CommunitiesCollectionState>
    {
        
        private readonly CommunityLinkFactory _links;

        public CommunitiesCollectionFactory(CommunityLinkFactory links)
        {
            _links = links;
        }
        public CommunitiesCollectionState Create(community model)
        {
            var community = new CommunitiesCollectionState
            {
                id = model.id,
                name = model.name,
                local = model.local,
                description = model.description,
                avatar = model.avatar,
                foundationDate = model.foundationDate.Value,
                _Links = new LinkCollection()
                    
            };
            //Add hyperMedia
            community._Links.self = _links.Self(model.id);
            return community;
        }
    
    }
}
