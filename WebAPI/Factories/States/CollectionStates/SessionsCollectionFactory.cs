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
    public class SessionsCollectionFactory : IStateFactory<session, SessionsCollectionState>
    {
        private readonly SessionLinkFactory _links;

        public SessionsCollectionFactory(SessionLinkFactory links)
        {
            _links = links;
        }

        public SessionsCollectionState Create(session model)
        {
            var session = new SessionsCollectionState { 
                id = model.id,
                description = model.description,
                initialDate = model.initialDate,
                endDate = model.endDate.Value,
                speaker = model.speakerName + " "+ model.lastName,
                title = model.title,
                profileSpeaker = model.linkOfSpeaker,
                _links = new LinkCollection()
            };
            session._links.self = _links.Self(model.id);
            return session;
        }
    }
}
