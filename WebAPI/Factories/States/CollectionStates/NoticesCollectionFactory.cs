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
    public class NoticesCollectionFactory : IStateFactory<notice, NoticesCollectionState>
    {
        private readonly NoticeLinkFactory _links;

        public NoticesCollectionFactory(NoticeLinkFactory links)
        {
            _links = links;
        }
        public NoticesCollectionState Create(notice model)
        {
            var notice = new NoticesCollectionState
            {
                id = model.id,
                title = model.title,
                description = model.description,
                date = model.initialDate.Value,
                _Links = new LinkCollection()
            };
            notice._Links.self = _links.Self(model.id);
            return notice;
        }
    }
}
