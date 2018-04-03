using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace WebAPI.Factories.States.SingleStates
{
    public class SurveyAnswerSingleFactory : IStateFactory<IEnumerable<surveyAnswer>, SurveyAnswerSingleState>
    {
        private readonly HttpRequestMessage _request;
        //private readonly SurveyLinkFactory _links;

        public SurveyAnswerSingleFactory(HttpRequestMessage request)
        {
            //_links = new SurveyLinkFactory(request);
            _request = request;
        }

        public SurveyAnswerSingleState Create(IEnumerable<surveyAnswer> model)
        {
            var a = new List<object>();
            var e = model.GroupBy(p => p.questionId).OrderBy(elem=> elem.Key);

            List<object> res = new List<object>();
            foreach (IGrouping<int,surveyAnswer> s in e)
            {
                var t = s.FirstOrDefault().survey.type;
                object r=null;
                if (t == "open_text")
                {
                    var response = s.Select(elem => elem.surveyTextAnswer.response).ToArray();
                    r = new { id = s.Key, question = s.FirstOrDefault().survey.question,type = t,response = response };
                }
                else
                {
                    List<surveyChoiceAnswer> list = new List<surveyChoiceAnswer>();
            
                    foreach (surveyAnswer sur in s) { list = list.Concat(sur.surveyChoiceAnswer).ToList(); }
                    var list2 =list.Select(elem => elem.surveyAnswer.survey.surveyChoice.Where(elem2 => elem.choiceId == elem2.choiceId).FirstOrDefault());
                    var result = list2.GroupBy(p => p.choiceId, (key, count) => new { id=key,count=count.Count(),answer=count.FirstOrDefault().message});
                    
                    r = new { id = s.Key, question = s.FirstOrDefault().survey.question, type = t, response = result,total= result.Sum(elem => elem.count) };
                }

                res.Add(r);
            }

            var survey = new SurveyAnswerSingleState
            {
                questions = res
                //_links = new UserSingleState.Link()
            };

            /* 
             * 
             * response_text = g.Select(ee=>ee.surveyTextAnswer.response).ToArray(),
                                response_choice = g.Select(ee => ee.surveyChoiceAnswer.GroupBy(p=>p.choiceId,(key,res)=> res.Count()).ToArray()).ToArray()
             * 
             survey._links.self = _links.Self(model.id);
             survey._links.subscribedEvents = _links.events(model.id);
             survey._links.admin = _links.CommunitiesWhereUserIsAdmin(model.id);
             survey._links.member = _links.CommunitiesWhereUserIsMember(model.id);
            */
            return survey;
        }
    }
}