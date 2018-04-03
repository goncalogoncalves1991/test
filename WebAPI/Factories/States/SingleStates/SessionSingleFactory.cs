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
    public class SessionSingleFactory :IStateFactory<session, SessionSingleState>
    {
        
        private readonly SessionLinkFactory _links;
        private readonly QuestionsCollectionFactory questionFactory;
        private readonly EventsCollectionFactory eventFactory;

        public SessionSingleFactory(HttpRequestMessage Request)
        {
            _links = new SessionLinkFactory(Request);
            questionFactory = new QuestionsCollectionFactory(new QuestionLinkFactory(Request));
            eventFactory = new EventsCollectionFactory(new EventLinkFactory(Request)); 
            
        }

        public SessionSingleState Create(session model)
        {
            var session = new SessionSingleState
            {
                id = model.id,
                description = model.description,
                initialDate = model.initialDate,
                endDate = model.endDate.Value,
                speaker = model.speakerName + " " + model.lastName,
                title = model.title,
                profileSpeaker = model.linkOfSpeaker,
                _event = eventFactory.Create(model.@event),
                questions = model.question.Select<question, QuestionsCollectionState>(i => questionFactory.Create(i)),
                _links = new SessionSingleState.Link()
            };
            session._links.self = _links.Self(model.id);
            session._links.questions = _links.Questions(model.id);
            session._links._event = _links.Event(model.eventId.Value);

            return session;
        }
    }
}
