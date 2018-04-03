using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using WebAPI.Factories.Links;
using WebAPI.Models.Singles;

namespace WebAPI.Factories.States.SingleStates
{
    public class SurveySingleFactory : IStateFactory<IEnumerable<surveyQuestion>, SurveySingleState>
    {
        private readonly HttpRequestMessage _request;
        private readonly SurveyLinkFactory _links;

        public SurveySingleFactory(HttpRequestMessage request)
        {
            _links = new SurveyLinkFactory(request);
            _request = request;
        }

        public SurveySingleState Create(IEnumerable<surveyQuestion> model)
        {
            var a = new List<object>();
            foreach (surveyQuestion sur in model)
            {
                if(sur.surveyChoice.Count()==0)  a.Add(new { question = sur.question, type=sur.type});
                else
                {
                    var b = new List<string>();
                    foreach(surveyChoice c in sur.surveyChoice)
                    {
                        b.Add(c.message);
                    }
                    a.Add(new { question = sur.question, type = sur.type, choices = b });
                }
            }

            var survey = new SurveySingleState
            {
                questions = a
                //_links = new UserSingleState.Link()
            };

            /* 
             survey._links.self = _links.Self(model.id);
             survey._links.subscribedEvents = _links.events(model.id);
             survey._links.admin = _links.CommunitiesWhereUserIsAdmin(model.id);
             survey._links.member = _links.CommunitiesWhereUserIsMember(model.id);
            */
            return survey;
        }
    }
}