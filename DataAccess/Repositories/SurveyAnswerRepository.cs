using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using static DataAccess.Models.Create.CreateSurveyAnswer;
using System.Data.SqlClient;

namespace DataAccess.Repositories
{
    public class SurveyAnswerRepository : Repository<surveyAnswer>, IRepository<CreateSurveyAnswer, surveyAnswer, int>
    {
        public Task<int> DeleteAsync(IEnumerable<surveyAnswer> item)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int res = 0;
                        foreach (surveyAnswer answer in item) { 
                            context.surveyAnswer.Remove(answer);
                            res = context.SaveChanges();
                        }
                        dbcxtransaction.Commit();
                        return res;
                    }
                    catch (SqlException ex)
                    {
                        throw new ArgumentException(ex.Message);
                    }
                }

            });
        }

        public Task<IEnumerable<surveyAnswer>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<surveyAnswer> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<surveyAnswer>> GetFromEventAsync(int eventId)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    var res =  (IEnumerable<surveyAnswer>)context.surveyAnswer
                        .Include(s => s.surveyTextAnswer)
                        .Include(s => s.survey.surveyChoice)
                        .Include(s => s.surveyChoiceAnswer)
                        .Include(s=> s.survey)
                        .Where(s => s.eventId == eventId)
                        .ToList();
                    dbcxtransaction.Commit();

                    return res;
                }
            });
        }

        public Task<int> PostAsync(CreateSurveyAnswer item)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (SurveyQuestionAnswer q in item.questions)
                        {
                            var result = new surveyAnswer()
                            {
                                eventId = item.eventId,
                                authorId = item.authorId,
                                questionId = q.questionId
                            };

                            context.surveyAnswer.Add(result);
                            context.SaveChanges();

                            InsertTextAnswer(item, q);

                            InsertChoicesAnswer(item, q);
                        }
                        dbcxtransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException(e.Message);
                    }
                    return 0;
                }
            });
        }       

        public Task<int> PutAsync(surveyAnswer item)
        {
            throw new NotImplementedException();
        }

        private void InsertChoicesAnswer(CreateSurveyAnswer item, SurveyQuestionAnswer q)
        {
            if (q.choices != null)
            {
                foreach (int c in q.choices)
                {
                    var choiceResult = new surveyChoiceAnswer()
                    {
                        authorId = item.authorId,
                        questionId = q.questionId,
                        eventId = item.eventId,
                        choiceId = c
                    };

                    context.surveyChoiceAnswer.Add(choiceResult);
                    context.SaveChanges();
                }
            }
        }

        private void InsertTextAnswer(CreateSurveyAnswer item, SurveyQuestionAnswer q)
        {
            if (q.text != null)
            {
                var textResult = new surveyTextAnswer()
                {
                    authorId = item.authorId,
                    questionId = q.questionId,
                    eventId = item.eventId,
                    response = q.text
                };

                context.surveyTextAnswer.Add(textResult);
                context.SaveChanges();
            }
        }

        public Task<bool> GetByIdAsync(int eventId, string authorId)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    var res = context.surveyAnswer
                        .Any(s => (s.eventId == eventId) && (s.authorId == authorId));
                    dbcxtransaction.Commit();

                    return res;
                }
            });
        }

        public Task<int> DeleteAsync(surveyAnswer item)
        {
            throw new NotImplementedException();
        }
    }
}
