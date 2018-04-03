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
    public class EventsCollectionFactory : IStateFactory<@event, EventsCollectionState>
    {
        private readonly EventLinkFactory _links;

        public EventsCollectionFactory(EventLinkFactory links)
        {
            _links = links;
        }
        public EventsCollectionState Create(@event model)
        {
            var eve = new EventsCollectionState
            {
                id = model.id,
                title = model.title,
                community = model.community!=null ? model.community.name:null,
                local = model.local,
                initDate = model.initDate,
                endDate = model.endDate,
                description = model.description,
                nrOfTickets = model.nrOfTickets.Value,
                _Links = new LinkCollection()
                    
            };
            //Add hyperMedia
            eve._Links.self = _links.Self(model.id);
            return eve;
        }
    }
}
